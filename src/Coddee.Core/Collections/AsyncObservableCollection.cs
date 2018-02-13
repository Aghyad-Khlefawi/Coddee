// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Coddee.Collections
{
    /// <summary>
    /// A thread-safe collection with change notifications.
    /// </summary>
    public interface IAsyncObservableCollection : ICollection
    {
        /// <summary>
        /// Add an object to the collection.
        /// </summary>
        void AddObject(object item);

        /// <summary>
        /// Add multiple objects to the collection.
        /// </summary>
        void AddRangeObject(IEnumerable<object> item);


        /// <summary>
        /// Clear the collection.
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// A simple implementation for an ObservableCollection that is thread safe.
    /// Useful for WPF controls that requires the UI thread to execute the INotifyCollectionChanged
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncObservableCollection<T> : Collection<T>, IAsyncObservableCollection, INotifyPropertyChanged, INotifyCollectionChanged
    {
        /// <summary>
        /// Create a new instance of <see cref="AsyncObservableCollection{T}"/>
        /// </summary>
        public static AsyncObservableCollection<T> Create()
        {
            AsyncObservableCollection<T> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableCollection<T>(); });
            return collection;
        }

        /// <summary>
        /// Create a new instance of <see cref="AsyncObservableCollection{T}"/>
        /// </summary>
        /// <param name="list">an existed collection to copy</param>
        public static AsyncObservableCollection<T> Create(IList<T> list)
        {
            AsyncObservableCollection<T> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableCollection<T>(list); });
            return collection;
        }

        /// <summary>
        /// Create a new instance of <see cref="AsyncObservableCollection{T}"/>
        /// </summary>
        /// <param name="list">an existed collection to copy</param>
        public static AsyncObservableCollection<T> Create(IEnumerable<T> list)
        {
            AsyncObservableCollection<T> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableCollection<T>(list); });
            return collection;
        }

        /// <inheritdoc/>
        public AsyncObservableCollection()
        {
        }

        /// <inheritdoc/>
        public AsyncObservableCollection(int size)
            : base(new List<T>(size))
        {
        }
        /// <inheritdoc/>
        public AsyncObservableCollection(IList<T> list)
            : base(list)
        {
        }
        /// <inheritdoc/>
        public AsyncObservableCollection(IEnumerable<T> list)
            : this()
        {
            CopyFrom(list);
        }

        /// <summary>
        /// Raised when the SelectedItem property setter is called
        /// Might send a null argument if no item was selected 
        /// </summary>
        public event EventHandler<T> SelectedItemChanged;

        private bool _isBusy;
        /// <summary>
        /// Indicates that the collection is filling its items.
        /// Can be bound to a busy indicator.
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        private T _selectedItem;

        /// <summary>
        /// Represent the selected item of the collection.
        /// Should be bound to the selected item property of the ListBox of DataGrid.
        /// Can be null is no item is selected
        /// </summary>
        public T SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                SelectedItemChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Copy the items from another collection
        /// </summary>
        /// <param name="collection">The collection to copy</param>
        protected void CopyFrom(IEnumerable<T> collection)
        {
            if (collection != null)
            {
                foreach (var item in collection)
                {
                    UnsafeAdd(item);
                }
            }
        }


        /// <summary>
        /// Execute an action on SynchornizationContext (supposedly the UI context)
        /// </summary>
        /// <param name="action">The action to execute</param>
        protected static void ExecuteOnSyncContext(Action action)
        {
            UISynchronizationContext.ExecuteOnUIContext(action);
        }


        /// <summary>
        /// Adds new item without using a lock
        /// </summary>
        /// <param name="item"></param>
        protected virtual void UnsafeAdd(T item)
        {
            UnsafeInsertItem(Items.Count, item);
        }

        /// <summary>
        /// Adds new item to the collections
        /// </summary>
        /// <param name="item"></param>
        public new void Add(T item)
        {
            ExecuteOnSyncContext(() =>
            {
                UnsafeInsertItem(Items.Count, item);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            });
        }

        /// <summary>
        /// Adds new item to the collections
        /// </summary>
        /// <param name="item"></param>
        private void AddSafe(T item)
        {
            UnsafeInsertItem(Items.Count, item);
        }

        /// <summary>
        /// Fill the collection from another collection
        /// </summary>
        /// <param name="collection">The other collection</param>
        public void Fill(IEnumerable<T> collection)
        {
            ExecuteOnSyncContext(() =>
            {
                if (collection == null || !collection.Any())
                    return;
                IsBusy = true;
                foreach (var item in collection)
                {
                    AddSafe(item);
                }
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                IsBusy = false;
            });
        }
        
        /// <summary>
        /// Fill the collection from another collection
        /// </summary>
        /// <param name="collection">The other collection</param>
        public void AddRange(IEnumerable<T> collection)
        {
            Fill(collection);
        }

        /// <summary>
        /// Clears the collection then fills the collection from another collection
        /// </summary>
        /// <param name="collection">The other collection</param>
        public void ClearAndFill(IEnumerable<T> collection)
        {
            Clear();
            Fill(collection);
        }

        /// <inheritdoc/>
        public new void Clear()
        {
            SelectedItem = default(T);
            base.Clear();
        }

        /// <summary>
        /// Fill the collection from a task result
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public async Task FillAsync(Task<IEnumerable<T>> collection)
        {
            Fill(await collection);
        }

        /// <summary>
        /// Fill the collection from a task result
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public async Task ClearAndFillAsync(Task<IEnumerable<T>> collection)
        {
            ClearAndFill(await collection);
        }

        /// <summary>
        /// Inserts an item without using the SynchornizationContext nor raising collection changed event
        /// </summary>
        /// <param name="index">The index in which the item will be inserted</param>
        /// <param name="item">The item to insert</param>
        protected virtual void UnsafeInsertItem(int index, T item)
        {
            base.InsertItem(index, item);
        }
        /// <inheritdoc/>
        protected override void InsertItem(int index, T item)
        {
            ExecuteOnSyncContext(() =>
            {
                UnsafeInsertItem(index, item);
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged("Item[]");
                OnCollectionChanged(
                                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                                                                         item,
                                                                         index));
            });
        }

        /// <summary>
        /// Removes an item from the collection in a thread safe way
        /// </summary>
        /// <param name="item">The item to insert</param>
        public new bool Remove(T item)
        {
            bool res = false;
            ExecuteOnSyncContext(() =>
            {
                int index = Items.IndexOf(item);
                if (index < 0)
                    res = false;
                RemoveItem(index);

                res = true;
            });
            return res;
        }

        /// <summary>
        /// Remove and item from the collection without using the SynchornizationContext nor raising collection changed event
        /// </summary>
        /// <param name="index">The index of the item that will be removed</param>
        protected virtual void UnsafeRemoveItem(int index)
        {
            base.RemoveItem(index);
        }

        /// <summary>
        /// Removes an item from the collection in thread a safe way
        /// </summary>
        /// <param name="index">The index of the item that will be removed</param>
        protected override void RemoveItem(int index)
        {
            ExecuteOnSyncContext(() =>
            {
                var item = this[index];
                base.RemoveItem(index);
                if (ReferenceEquals(SelectedItem, item))
                    SelectedItem = default(T);
                OnPropertyChanged("Item[]");
                OnCollectionChanged(
                                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                                                                         item,
                                                                         index));
            });
        }

        /// <summary>
        /// Clears the collection from items in a thread safe way
        /// </summary>
        protected override void ClearItems()
        {
            ExecuteOnSyncContext(() =>
            {
                base.ClearItems();
                SelectedItem = default(T);
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged("Item[]");
                OnCollectionReset();
            });
        }

        /// <summary>
        /// Moves and item in the collection from one position to another in a thread safe way
        /// </summary>
        /// <param name="oldIndex">The item old index</param>
        /// <param name="newIndex">The item new index</param>
        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            ExecuteOnSyncContext(() =>
            {
                T obj = this[oldIndex];
                UnsafeRemoveItem(oldIndex);
                UnsafeInsertItem(newIndex, obj);
                OnPropertyChanged("Item[]");
                OnCollectionChanged(NotifyCollectionChangedAction.Move, obj, newIndex, oldIndex);
            });
        }


        /// <summary>
        /// Replaces the element at the specified index. in a thread safe way
        /// </summary>
        /// <param name="index">The index of the item to be replaced</param>
        /// <param name="item">The new item</param>
        protected override void SetItem(int index, T item)
        {
            ExecuteOnSyncContext(() =>
            {
                T obj = this[index];
                base.SetItem(index, item);
                OnPropertyChanged("Item[]");
                OnCollectionChanged(NotifyCollectionChangedAction.Replace, obj, item, index);
            });
        }


        //Property changed and collection changed implementations 
        /// <inheritdoc cref="INotifyPropertyChanged"/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc cref="INotifyCollectionChanged"/>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
        }

        private void OnCollectionChanged(
            NotifyCollectionChangedAction action,
            object oldItem,
            object newItem,
            int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        private void OnCollectionReset()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <inheritdoc cref="INotifyCollectionChanged"/>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            ExecuteOnSyncContext(() => { CollectionChanged?.Invoke(this, e); });
        }

        /// <inheritdoc/>
        public void AddObject(object item)
        {
            Add((T)item);
        }

        /// <inheritdoc/>
        public void AddRangeObject(IEnumerable<object> item)
        {
            Fill(item.Cast<T>());
        }
    }
}