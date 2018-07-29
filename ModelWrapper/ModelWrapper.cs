using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Utilities;

namespace ModelWrapper
{
    /// <inheritdoc>
    ///     <cref></cref>
    /// </inheritdoc>
    /// <summary>
    /// Base class of Model Wrappers.
    /// Performs change-notification and Provides services for change-tracking.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract partial class ModelWrapper<T> : Observable, IRevertibleChangeTracking
    {
        // Simple property change tracking
        private readonly Dictionary<string, dynamic> _originalValues;
        // Complex property change tracking
        private readonly List<IRevertibleChangeTracking> _trackedObjects;

        protected ModelWrapper(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            Model = model;
            _originalValues = new Dictionary<string, dynamic>();
            _trackedObjects = new List<IRevertibleChangeTracking>();
        }

        public T Model { get; }

        /// <summary>
        /// Set the value of the underlying model property.
        /// </summary>
        /// <typeparam name="TValue">Property type</typeparam>
        /// <param name="newValue">New value for property</param>
        /// <param name="propertyName">Name of property</param>
        protected void SetValue<TValue>(TValue newValue, [CallerMemberName] string propertyName = null)
        {
            var propertyInfo = Model.GetType().GetProperty(propertyName);
            var previousValue = propertyInfo?.GetValue(Model);

            if (Equals(previousValue, newValue)) return;

            // Change tracking of simple properties
            UpdateOriginalValue(previousValue, newValue, propertyName);

            propertyInfo?.SetValue(Model, newValue);

            // Change notification for property binding
            OnPropertyChanged(propertyName);
            OnPropertyChanged($"{propertyName}IsChanged");
        }

        /// <summary>
        /// Returns true if the value for the (simple) property has been changed.
        /// </summary>
        /// <param name="propertyName">Underlying model property name</param>
        /// <returns>[true | false]</returns>
        protected bool GetIsChanged(string propertyName)
        {
            return _originalValues.ContainsKey(propertyName);
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns true if any property value (simple, complex or collection-element) has changed.
        /// </summary>
        public bool IsChanged => _originalValues.Count > 0 || _trackedObjects.Any(o => o.IsChanged);


        /// <inheritdoc />
        /// <summary>
        /// For each complex object, recursively accept all changes to simple properties.
        /// </summary>
        public void AcceptChanges()
        {
            _originalValues.Clear();
            foreach (var trackedObject in _trackedObjects)
            {
                trackedObject.AcceptChanges();
            }

            // Change notification to all bindings
            OnPropertyChanged(null);
        }

        /// <summary>
        /// For each complex object, recursely revert all changes to simple properties to their initial value.
        /// </summary>
        public void RejectChanges()
        {
            foreach (var originalValue in _originalValues)
            {
                typeof(T).GetProperty(originalValue.Key).SetValue(Model, originalValue.Value);
            }
            _originalValues.Clear();
            foreach (var revertibleChangeTracking in _trackedObjects)
            {
                revertibleChangeTracking.RejectChanges();
            }

            // Change notification to all bindings
            OnPropertyChanged(null);
        }

        /// <summary>
        /// Get the value of the underlying model property using reflection.
        /// </summary>
        /// <typeparam name="TValue">Property data type</typeparam>
        /// <param name="propertyName">Name of property</param>
        /// <returns>Pre-existing value</returns>
        protected TValue GetValue<TValue>([CallerMemberName] string propertyName = null)
        {
            var propertyInfo = Model.GetType().GetProperty(propertyName);
            return (TValue)propertyInfo?.GetValue(Model);
        }

        /// <summary>
        /// Get original value of the underlying model property, before any changes.
        /// </summary>
        /// <typeparam name="TValue">Property data type</typeparam>
        /// <param name="propertyName">Name of property</param>
        /// <returns>Original value</returns>
        protected TValue GetOriginalValue<TValue>(string propertyName)
        {
            // original value is either in the change-tracking dictionary, or the underlying model property
            return _originalValues.ContainsKey(propertyName) ? (TValue)_originalValues[propertyName] : GetValue<TValue>(propertyName);
        }

        /// <summary>
        /// Keep underlying model collection synchronized with wrapper-collection.
        /// Note: wrapper-collection performs change-notification and change-tracking.
        /// </summary>
        /// <typeparam name="TWrapper">Wrapper class</typeparam>
        /// <typeparam name="TModel">Underlying class</typeparam>
        /// <param name="wrapperCollection">ChangeTrackingCollection</param>
        /// <param name="modelCollection">Underlying generic collection</param>
        protected void RegisterCollection<TWrapper, TModel>(
            ChangeTrackingCollection<TWrapper> wrapperCollection,
            List<TModel> modelCollection) where TWrapper : ModelWrapper<TModel>
        {
            // Synchronize underlying model collection with wrapperCollection
            wrapperCollection.CollectionChanged += (s, e) =>
            {
                modelCollection.Clear();
                modelCollection.AddRange(wrapperCollection.Select(w => w.Model));
            };
            RegisterTrackingObject(wrapperCollection);
        }

        /// <summary>
        /// Set complex property for change-tracking.
        /// </summary>
        /// <typeparam name="TModel">Complex class</typeparam>
        /// <param name="trackingObject">Complex value</param>
        protected void RegisterComplexOject<TModel>(ModelWrapper<TModel> trackingObject)
        {
            RegisterTrackingObject(trackingObject);
        }
    }
}
