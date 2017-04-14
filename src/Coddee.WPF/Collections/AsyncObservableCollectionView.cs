using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Coddee.Collections;

namespace Coddee.WPF.Collections
{
    public class AsyncObservableCollectionView<T> : AsyncObservableCollection<T>
    {
        public static AsyncObservableCollectionView<T> Create(Func<T, string, bool> filterPredicate)
        {
            AsyncObservableCollectionView<T> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableCollectionView<T>(); });
            collection.FilterItem = filterPredicate;
            return collection;
        }

        public static AsyncObservableCollectionView<T> Create(Func<T, string, bool> filterPredicate,IList<T> list)
        {
            AsyncObservableCollectionView<T> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableCollectionView<T>(list); });
            collection.FilterItem = filterPredicate;
            return collection;
        }

        public static AsyncObservableCollectionView<T> Create(Func<T, string, bool> filterPredicate, IEnumerable<T> list)
        {
            AsyncObservableCollectionView<T> collection = null;
            ExecuteOnSyncContext(() => { collection = new AsyncObservableCollectionView<T>(list); });
            collection.FilterItem = filterPredicate;
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
            CollectionView.Filter = e => FilterItem((T) e, search);
        }

    }
}