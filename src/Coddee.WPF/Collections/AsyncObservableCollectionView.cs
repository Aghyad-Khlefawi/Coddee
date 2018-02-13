// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

namespace Coddee.Collections
{
    /// <summary>
    /// A delegate that represents a filter handler
    /// </summary>
    /// <typeparam name="T">The filtered item type</typeparam>
    /// <param name="item">The filtered item</param>
    /// <param name="term">The filter term</param>
    /// <returns></returns>
    public delegate bool FilterHandler<T>(T item, string term);

    /// <summary>
    /// An <see cref="AsyncObservableCollection{T}"/> that provides an additional <see cref="ICollectionView"/> property for filtering and grouping.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncObservableCollectionView<T> : AsyncObservableCollection<T>
    {

        /// <summary>
        /// Creates a new instance of <see cref="AsyncObservableCollectionView{T}"/>
        /// </summary>
        /// <param name="filterPredicate">The filter handler</param>
        public static AsyncObservableCollectionView<T> Create(FilterHandler<T> filterPredicate)
        {
            AsyncObservableCollectionView<T> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableCollectionView<T>(); });
            if (filterPredicate != null)
                collection.FilterItem = (item, term) => filterPredicate(item, term);
            return collection;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AsyncObservableCollectionView{T}"/>
        /// </summary>
        /// <param name="list">an existed collection to copy</param>
        /// <param name="filterPredicate">The filter handler</param>
        public static AsyncObservableCollectionView<T> Create(FilterHandler<T> filterPredicate, IList<T> list)
        {
            AsyncObservableCollectionView<T> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableCollectionView<T>(list); });
            if (filterPredicate != null)
                collection.FilterItem = (item, term) => filterPredicate(item, term);
            return collection;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AsyncObservableCollectionView{T}"/>
        /// </summary>
        /// <param name="list">an existed collection to copy</param>
        /// <param name="filterPredicate">The filter handler</param>
        public static AsyncObservableCollectionView<T> Create(FilterHandler<T> filterPredicate, IEnumerable<T> list)
        {
            AsyncObservableCollectionView<T> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableCollectionView<T>(list); });
            if (filterPredicate != null)
                collection.FilterItem = (item, term) => filterPredicate(item, term);
            return collection;
        }

        /// <inheritdoc/>
        public AsyncObservableCollectionView()
        {
        }

        /// <inheritdoc/>
        public AsyncObservableCollectionView(IList<T> list)
            : base(list)
        {
        }

        /// <inheritdoc/>
        public AsyncObservableCollectionView(IEnumerable<T> list)
            : base(list)
        {

        }

        /// <summary>
        /// The current filter.
        /// </summary>
        public Func<T, string, bool> FilterItem { get; set; }

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
            var search = searchValue?.ToLower() ?? string.Empty;
            CollectionView.Filter = e => FilterItem((T)e, search);
        }

    }
}