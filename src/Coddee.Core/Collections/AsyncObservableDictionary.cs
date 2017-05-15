// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.Collections
{
    /// <summary>
    /// A similar collection to AsyncObservableCollection with the addition of a dictionary to lookup items
    /// </summary>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <typeparam name="TValue">Type value type</typeparam>
    public class AsyncObservableDictionary<TKey, TValue> : AsyncObservableCollection<TValue>
        where TValue : IUniqueObject<TKey>
    {

        public new static AsyncObservableDictionary<TKey,TValue> Create()
        {
            AsyncObservableDictionary<TKey, TValue> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableDictionary<TKey, TValue>(); });
            return collection;
        }

        public new static AsyncObservableDictionary<TKey, TValue> Create(IList<TValue> list)
        {
            AsyncObservableDictionary<TKey, TValue> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableDictionary<TKey, TValue>(list); });
            return collection;
        }

        public new static AsyncObservableDictionary<TKey, TValue> Create(IEnumerable<TValue> list)
        {
            AsyncObservableDictionary<TKey, TValue> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableDictionary<TKey, TValue>(list); });
            return collection;
        }

        public AsyncObservableDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        public AsyncObservableDictionary(IList<TValue> list)
            : base(list)
        {
            _dictionary = new Dictionary<TKey, TValue>();
            FillDicionaryValues(list);
        }


        public AsyncObservableDictionary(IEnumerable<TValue> list)
            : base(list)
        {
            _dictionary = new Dictionary<TKey, TValue>();
            FillDicionaryValues(list);
        }

        /// <summary>
        /// The dictionary for the items lookup
        /// </summary>
        private Dictionary<TKey, TValue> _dictionary;


        /// <summary>
        /// The indexer to retrieve the item by its key
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TValue this[TKey index] => _dictionary[index];

        /// <summary>
        /// Clears the collection from items in a thread safe way
        /// </summary>
        protected override void ClearItems()
        {
            base.ClearItems();
            ExecuteOnSyncContext(_dictionary.Clear);
        }

        /// <summary>
        /// Inserts an item without using the SynchornizationContext nor raising collection changed event
        /// </summary>
        /// <param name="index">The index in which the item will be inserted</param>
        /// <param name="item">The item to insert</param>
        protected override void UnsafeInsertItem(int index, TValue item)
        {
            base.UnsafeInsertItem(index, item);
            if(_dictionary==null)
                _dictionary = new Dictionary<TKey, TValue>();
            _dictionary.Add(item.GetKey, item);
        }

        /// <summary>
        /// Remove and item from the collection without using the SynchornizationContext nor raising collection changed event
        /// </summary>
        /// <param name="index">The index of the item that will be removed</param>
        protected override void UnsafeRemoveItem(int index)
        {
            TValue item = Items.ElementAt(index);
            base.UnsafeRemoveItem(index);
            _dictionary.Remove(item.GetKey);
        }

        /// <summary>
        /// Fills the dictionary with the keys and values
        /// </summary>
        /// <param name="collection">The collection to copy from</param>
        private void FillDicionaryValues(IEnumerable<TValue> collection)
        {
            _dictionary = new Dictionary<TKey, TValue>(collection.Count());
            foreach (var item in collection)
                _dictionary[item.GetKey] = item;
        }
    }
}