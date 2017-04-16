// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Coddee.Collections
{
    /// <summary>
    /// A simple implementation for an ObservableCollection that is thread safe.
    /// Useful for WPF controls that requires the UI thread to execute the INotifyCollectionChanged
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncObservableCollection<T> : Collection<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {

        public static AsyncObservableCollection<T> Create()
        {
            AsyncObservableCollection<T> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableCollection<T>(); });
            return collection;
        }

        public static AsyncObservableCollection<T> Create(IList<T> list)
        {
            AsyncObservableCollection<T> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableCollection<T>(list); });
            return collection;
        }

        public static AsyncObservableCollection<T> Create(IEnumerable<T> list)
        {
            AsyncObservableCollection<T> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableCollection<T>(list); });
            return collection;
        }

        public AsyncObservableCollection()
        {
        }

        public AsyncObservableCollection(IList<T> list)
            : base(list)
        {
        }

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

        /// <summary>
        /// Indicates that the collection is filling its items.
        /// Can be bound to a busy indicator.
        /// </summary>
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Represent the selected item of the collection.
        /// Should be bound to the selected item property of the ListBox of DataGrid.
        /// Might be null
        /// </summary>
        private T _selectedItem;
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
        /// Fill the collection from another collection
        /// </summary>
        /// <param name="collection">The other collection</param>
        public void Fill(IEnumerable<T> collection)
        {
            if (collection == null || !collection.Any()) return;
            IsBusy = true;
            foreach (var item in collection)
            {
                Add(item);
            }
            IsBusy = false;
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
        /// Inserts an item without using the SynchornizationContext nor raising collection changed event
        /// </summary>
        /// <param name="index">The index in which the item will be inserted</param>
        /// <param name="item">The item to insert</param>
        protected virtual void UnsafeInsertItem(int index, T item)
        {
            base.InsertItem(index, item);
        }

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
            int index = Items.IndexOf(item);
            if (index < 0)
                return false;
            RemoveItem(index);
            return true;
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            ExecuteOnSyncContext(() => { CollectionChanged?.Invoke(this, e); });
        }
    }
}