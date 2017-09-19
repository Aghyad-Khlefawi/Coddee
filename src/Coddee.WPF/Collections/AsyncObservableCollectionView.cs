// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

namespace Coddee.Collections
{
    public delegate bool FilterHandler<T>(T item, string term);

    public class AsyncObservableCollectionView<T> : AsyncObservableCollection<T>
    {


        public static AsyncObservableCollectionView<T> Create(FilterHandler<T> filterPredicate)
        {
            AsyncObservableCollectionView<T> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableCollectionView<T>(); });
            if (filterPredicate != null)
                collection.FilterItem = (item, term) => filterPredicate(item, term);
            return collection;
        }

        public static AsyncObservableCollectionView<T> Create(FilterHandler<T> filterPredicate, IList<T> list)
        {
            AsyncObservableCollectionView<T> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableCollectionView<T>(list); });
            if (filterPredicate != null)
                collection.FilterItem = (item, term) => filterPredicate(item, term);
            return collection;
        }

        public static AsyncObservableCollectionView<T> Create(FilterHandler<T> filterPredicate, IEnumerable<T> list)
        {
            AsyncObservableCollectionView<T> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableCollectionView<T>(list); });
            if (filterPredicate != null)
                collection.FilterItem = (item, term) => filterPredicate(item, term);
            return collection;
        }

        public AsyncObservableCollectionView()
        {
        }

        public AsyncObservableCollectionView(IList<T> list)
            : base(list)
        {
        }

        public AsyncObservableCollectionView(IEnumerable<T> list)
            : base(list)
        {

        }

        public Func<T, string, bool> FilterItem { get; set; }

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
            CollectionView.Filter = e => FilterItem((T)e, search);
        }

    }
}