using System.ComponentModel;
using Utilities;

namespace ModelWrapper
{
    public abstract partial class ModelWrapper<T> : Observable, IRevertibleChangeTracking
    {
        /// <summary>
        /// If changed, store value in change-tracking collection;
        /// otherwise, if it is once again equal to the original value, remove from change-tracking collection.
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        private void UpdateOriginalValue(object currentValue, object newValue, string propertyName)
        {
            // if an original_value has not yet been stored for this property...
            if (!_originalValues.ContainsKey(propertyName))
            {
                _originalValues.Add(propertyName, currentValue);
                OnPropertyChanged(nameof(IsChanged));
            }
            // otherwise, if the new_value is once again the same as the original value...
            else if (Equals(_originalValues[propertyName], newValue))
            {
                _originalValues.Remove(propertyName);
                OnPropertyChanged(nameof(IsChanged));
            }
        }

        /// <summary>
        /// Register complex member for change-tracking and ensure change-notification is perfomed.
        /// </summary>
        /// <typeparam name="TTrackingObject"></typeparam>
        /// <param name="trackingObject"></param>
        private void RegisterTrackingObject<TTrackingObject>(TTrackingObject trackingObject)
            where TTrackingObject : IRevertibleChangeTracking, INotifyPropertyChanged
        {
            if (_trackedObjects.Contains(trackingObject)) return;

            _trackedObjects.Add(trackingObject);
            trackingObject.PropertyChanged += TrackingObjectPropertyChanged;
        }


        private void TrackingObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Equals(e.PropertyName, nameof(IsChanged)))
            {
                OnPropertyChanged(nameof(IsChanged));
            }
        }
    }
}
