// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using Coddee.Collections;

namespace Coddee.WPF.Collections
{
    /// <summary>
    /// An <see cref="AsyncObservableDictionaryView{TKey,TValue}"/> implementation that provides an <see cref="ICollectionView"/>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class AsyncObservableDictionaryView<TKey, TValue> : AsyncObservableDictionary<TKey,TValue>
        where TValue : IUniqueObject<TKey>
    {
        /// <summary>
        /// Creates a new instance of <see cref="AsyncObservableDictionaryView{TKey,TValue}"/>
        /// </summary>
        /// <param name="filterPredicate">The filter handler</param>
        public static AsyncObservableDictionaryView<TKey, TValue> Create(Func<TValue, string, bool> filterPredicate)
        {
            AsyncObservableDictionaryView<TKey, TValue> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableDictionaryView<TKey, TValue>(); });
            collection.FilterItem = filterPredicate;
            return collection;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AsyncObservableDictionaryView{TKey,TValue}"/>
        /// </summary>
        /// <param name="list">an existed collection to copy</param>
        /// <param name="filterPredicate">The filter handler</param>
        public static AsyncObservableDictionaryView<TKey, TValue> Create(Func<TValue, string, bool> filterPredicate,IList<TValue> list)
        {
            AsyncObservableDictionaryView<TKey, TValue> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableDictionaryView<TKey, TValue>(list); });
            collection.FilterItem = filterPredicate;
            return collection;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AsyncObservableDictionaryView{TKey,TValue}"/>
        /// </summary>
        /// <param name="list">an existed collection to copy</param>
        /// <param name="filterPredicate">The filter handler</param>
        public static AsyncObservableDictionaryView<TKey, TValue> Create(Func<TValue, string, bool> filterPredicate, IEnumerable<TValue> list)
        {
            AsyncObservableDictionaryView<TKey, TValue> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableDictionaryView<TKey, TValue>(list); });
            collection.FilterItem = filterPredicate;
            return collection;
        }

        /// <inheritdoc />
        public AsyncObservableDictionaryView()
        {
        }

        /// <inheritdoc />
        public AsyncObservableDictionaryView(IList<TValue> list)
            : base(list)
        {
        }

        /// <inheritdoc />
        public AsyncObservableDictionaryView(IEnumerable<TValue> list)
            : base(list)
        {

        }


        /// <summary>
        /// The current filter.
        /// </summary>
        public Func<TValue, string, bool> FilterItem { get; set; }

        private ICollectionView _collectionView;
        /// <summary>
        /// <see cref="ICollectionView"/> object used for filtering and grouping.
        /// </summary>
        public ICollectionView CollectionView
        {
            get { return (_collectionView ?? (_collectionView = CollectionViewSource.GetDefaultView(this))); }
            set
            {
                _collectionView = value;
                OnPropertyChanged();
            }
        }

        private string _searchText;
        /// <summary>
        /// The current filter term.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged();
                Search(value);
            }
        }

        /// <summary>
        /// filter the collection.
        /// </summary>
        /// <param name="searchValue">The filter term.</param>
        public void Search(string searchValue)
        {
            var search = searchValue.ToLower();
            CollectionView.Filter = e => FilterItem((TValue) e, search);
        }

    }
}