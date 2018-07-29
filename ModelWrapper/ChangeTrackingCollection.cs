using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace ModelWrapper
{
    public class ChangeTrackingCollection<T> : ObservableCollection<T>, IRevertibleChangeTracking
    where T : IRevertibleChangeTracking, INotifyPropertyChanged
    {
        private IList<T> _originalCollection;

        // Change-monitoring collections.
        private readonly ObservableCollection<T> _addedItems;
        private readonly ObservableCollection<T> _removedItems;
        private readonly ObservableCollection<T> _modifiedItems;

        public ChangeTrackingCollection(IEnumerable<T> items) : base(items)
        {

            _originalCollection = this.ToList();

            AttachItemPropertChangedHandler(_originalCollection);

            _addedItems = new ObservableCollection<T>();
            _removedItems = new ObservableCollection<T>();
            _modifiedItems = new ObservableCollection<T>();

            AddedItems = new ReadOnlyObservableCollection<T>(_addedItems);
            RemovedItems = new ReadOnlyObservableCollection<T>(_removedItems);
            ModifiedItems = new ReadOnlyObservableCollection<T>(_modifiedItems);
        }

        /// <summary>
        /// Change monitoring collections are read-only to the outside.
        /// </summary>
        public ReadOnlyObservableCollection<T> AddedItems { get; }
        public ReadOnlyObservableCollection<T> RemovedItems { get; }
        public ReadOnlyObservableCollection<T> ModifiedItems { get; }

        /// <summary>
        /// Returns whether this ChangeTrackingCollection has changed.
        /// </summary>
        public bool IsChanged => AddedItems.Count > 0 || RemovedItems.Count > 0 || ModifiedItems.Count > 0;

        /// <summary>
        /// Invoke AcceptChanges method on all (IChangeTraking) items in this collection,
        /// and clear all monitoring collections.
        /// </summary>
        public void AcceptChanges()
        {
            _addedItems.Clear();
            _modifiedItems.Clear();
            _removedItems.Clear();
            foreach (var item in this)
            {
                item.AcceptChanges();
            }
            _originalCollection = this.ToList();
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
        }

        /// <summary>
        /// Undo all changes to this collection. This includes changes to member elements.
        /// </summary>
        public void RejectChanges()
        {
            foreach (var addedItem in _addedItems.ToList())
            {
                Remove(addedItem);
            }
            foreach (var modifiedItem in _modifiedItems.ToList())
            {
                modifiedItem.RejectChanges();
            }
            foreach (var removedItem in _removedItems.ToList())
            {
                Add(removedItem);
            }
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
        }

        /// <summary>
        /// What happens inside this collection when a member item is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = (T)sender;
            // If the object property changes follow addition to this collection,
            // then this collections is already considered changed by virtue of its new addition.
            if (_addedItems.Contains(item)) return;

            if (item.IsChanged)
            {
                if (!_modifiedItems.Contains(item))
                    _modifiedItems.Add(item);
            }
            else
            {
                if (_modifiedItems.Contains(item))
                    _modifiedItems.Remove(item);
            }
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var added = this.Where(current => _originalCollection.All(orig => !Equals(orig, current))).ToList();
            var removed = _originalCollection.Where(orig => this.All(current => !Equals(current, orig))).ToList();
            var modified = this.Except(added).Except(removed).Where(item => item.IsChanged).ToList();

            AttachItemPropertChangedHandler(added);
            DetachItemPropertChangedHandler(removed);

            UpdateObservableCollection(_addedItems, added);
            UpdateObservableCollection(_modifiedItems, modified);
            UpdateObservableCollection(_removedItems, removed);

            base.OnCollectionChanged(e);
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
        }

        /// <summary>
        /// Add PropertyChanged event handler to each item.
        /// </summary>
        /// <param name="collection"></param>
        private void AttachItemPropertChangedHandler(IList<T> collection)
        {
            foreach (var item in collection)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        /// <summary>
        /// Remove PropertyChanged event handler for each item.
        /// </summary>
        /// <param name="collection"></param>
        private void DetachItemPropertChangedHandler(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }

        /// <summary>
        /// Synchronize ObservableCollection with collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="items"></param>
        private void UpdateObservableCollection(ObservableCollection<T> collection, IEnumerable<T> items)
        {
            collection.Clear();
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}
