// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using Coddee.Collections;

namespace Coddee.WPF.Collections
{
    public class AsyncObservableDictionaryView<TKey, TValue> : AsyncObservableDictionary<TKey,TValue>
        where TValue : IUniqueObject<TKey>
    {
        public static AsyncObservableDictionaryView<TKey, TValue> Create(Func<TValue, string, bool> filterPredicate)
        {
            AsyncObservableDictionaryView<TKey, TValue> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableDictionaryView<TKey, TValue>(); });
            collection.FilterItem = filterPredicate;
            return collection;
        }

        public static AsyncObservableDictionaryView<TKey, TValue> Create(Func<TValue, string, bool> filterPredicate,IList<TValue> list)
        {
            AsyncObservableDictionaryView<TKey, TValue> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableDictionaryView<TKey, TValue>(list); });
            collection.FilterItem = filterPredicate;
            return collection;
        }

        public static AsyncObservableDictionaryView<TKey, TValue> Create(Func<TValue, string, bool> filterPredicate, IEnumerable<TValue> list)
        {
            AsyncObservableDictionaryView<TKey, TValue> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableDictionaryView<TKey, TValue>(list); });
            collection.FilterItem = filterPredicate;
            return collection;
        }

        public AsyncObservableDictionaryView()
        {
        }

        public AsyncObservableDictionaryView(IList<TValue> list)
            : base(list)
        {
        }

        public AsyncObservableDictionaryView(IEnumerable<TValue> list)
            : base(list)
        {

        }

        public Func<TValue, string, bool> FilterItem { get; set; }

        private ICollectionView _collectionView;
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

        public void Search(string searchValue)
        {
            var search = searchValue.ToLower();
            CollectionView.Filter = e => FilterItem((TValue) e, search);
        }

    }
}