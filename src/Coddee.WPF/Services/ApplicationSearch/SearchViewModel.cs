// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Coddee.Collections;
using Coddee.WPF;
using Coddee.WPF.Commands;

namespace Coddee.Services.ApplicationSearch
{
    /// <summary>
    /// Default implementation of <see cref="IApplicationQuickSearch"/>
    /// </summary>
    public class SearchViewModel : ViewModelBase<SearchView>, IApplicationQuickSearch
    {
        private readonly IApplicationSearchService _applicationSearchService;

        /// <inheritdoc />
        public SearchViewModel()
        {

        }
        /// <inheritdoc />
        public SearchViewModel(IApplicationSearchService applicationSearchService)
        {
            _applicationSearchService = applicationSearchService;
        }

        private bool _showPopup;

        /// <summary>
        /// Indicates whether the popup is displayed
        /// </summary>
        public bool ShowPopup
        {
            get { return _showPopup; }
            set { SetProperty(ref this._showPopup, value); }
        }

        private string _searchText;

        /// <summary>
        /// The search text of the service.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                SetProperty(ref this._searchText, value);
                SearchTextChanged(value);
            }
        }



        private AsyncObservableCollectionView<SearchItem> _searchResults;

        /// <summary>
        /// The results of the current search operation.
        /// </summary>
        public AsyncObservableCollectionView<SearchItem> SearchResults
        {
            get { return _searchResults; }
            set { SetProperty(ref this._searchResults, value); }
        }

        private SearchItem _selectedResult;

        /// <summary>
        /// Selected result from the popup.
        /// </summary>
        public SearchItem SelectedResult
        {
            get { return _selectedResult; }
            set { SetProperty(ref this._selectedResult, value); }
        }

        private bool _searchResultsFound = true;

        /// <summary>
        /// If false then no result were found in the search operation.
        /// </summary>
        public bool SearchResultsFound
        {
            get { return _searchResultsFound; }
            set { SetProperty(ref this._searchResultsFound, value); }
        }

        private SearchOperation _lastOperation;

        /// <summary>
        /// Close the search popup.
        /// </summary>
        public ICommand CloseSearchCommand => new RelayCommand(CloseSearch);

        /// <summary>
        /// Called when a key is pressed in the search bar.
        /// </summary>
        public ICommand KeyDownCommand => new RelayCommand<KeyEventArgs>(KeyDown);

        /// <summary>
        /// Called when a result is pressed.
        /// </summary>
        public ICommand NavigateCommand => new RelayCommand(Navigate);

        /// <summary>
        /// Called when the first result is selected.
        /// </summary>
        public ICommand NavigateFirstCommand => new RelayCommand(NavigateFirst);

        private void NavigateFirst()
        {
            Navigate();
        }

        private void Navigate()
        {
            if (SelectedResult == null && SearchResults != null && SearchResults.Any())
                SelectedResult = SearchResults.First();
            SelectedResult?.Navigate();
        }
        private void KeyDown(KeyEventArgs arg)
        {
            if (arg.Key == Key.Escape)
                CloseSearch();
        }


        /// <inheritdoc />
        protected override void OnDesignMode()
        {
            base.OnDesignMode();
            ShowPopup = true;
            SearchResults = new AsyncObservableCollectionView<SearchItem>()
            {
                new SearchItem(0,"Items1","Warehouse","Management","",Geometry.Parse("M12,15.5A3.5,3.5 0 0,1 8.5,12A3.5,3.5 0 0,1 12,8.5A3.5,3.5 0 0,1 15.5,12A3.5,3.5 0 0,1 12,15.5M19.43,12.97C19.47,12.65 19.5,12.33 19.5,12C19.5,11.67 19.47,11.34 19.43,11L21.54,9.37C21.73,9.22 21.78,8.95 21.66,8.73L19.66,5.27C19.54,5.05 19.27,4.96 19.05,5.05L16.56,6.05C16.04,5.66 15.5,5.32 14.87,5.07L14.5,2.42C14.46,2.18 14.25,2 14,2H10C9.75,2 9.54,2.18 9.5,2.42L9.13,5.07C8.5,5.32 7.96,5.66 7.44,6.05L4.95,5.05C4.73,4.96 4.46,5.05 4.34,5.27L2.34,8.73C2.21,8.95 2.27,9.22 2.46,9.37L4.57,11C4.53,11.34 4.5,11.67 4.5,12C4.5,12.33 4.53,12.65 4.57,12.97L2.46,14.63C2.27,14.78 2.21,15.05 2.34,15.27L4.34,18.73C4.46,18.95 4.73,19.03 4.95,18.95L7.44,17.94C7.96,18.34 8.5,18.68 9.13,18.93L9.5,21.58C9.54,21.82 9.75,22 10,22H14C14.25,22 14.46,21.82 14.5,21.58L14.87,18.93C15.5,18.67 16.04,18.34 16.56,17.94L19.05,18.95C19.27,19.03 19.54,18.95 19.66,18.73L21.66,15.27C21.78,15.05 21.73,14.78 21.54,14.63L19.43,12.97Z"), null),
                new SearchItem(1,"Item2","Warehouse","Management","",Geometry.Parse("M12,15.5A3.5,3.5 0 0,1 8.5,12A3.5,3.5 0 0,1 12,8.5A3.5,3.5 0 0,1 15.5,12A3.5,3.5 0 0,1 12,15.5M19.43,12.97C19.47,12.65 19.5,12.33 19.5,12C19.5,11.67 19.47,11.34 19.43,11L21.54,9.37C21.73,9.22 21.78,8.95 21.66,8.73L19.66,5.27C19.54,5.05 19.27,4.96 19.05,5.05L16.56,6.05C16.04,5.66 15.5,5.32 14.87,5.07L14.5,2.42C14.46,2.18 14.25,2 14,2H10C9.75,2 9.54,2.18 9.5,2.42L9.13,5.07C8.5,5.32 7.96,5.66 7.44,6.05L4.95,5.05C4.73,4.96 4.46,5.05 4.34,5.27L2.34,8.73C2.21,8.95 2.27,9.22 2.46,9.37L4.57,11C4.53,11.34 4.5,11.67 4.5,12C4.5,12.33 4.53,12.65 4.57,12.97L2.46,14.63C2.27,14.78 2.21,15.05 2.34,15.27L4.34,18.73C4.46,18.95 4.73,19.03 4.95,18.95L7.44,17.94C7.96,18.34 8.5,18.68 9.13,18.93L9.5,21.58C9.54,21.82 9.75,22 10,22H14C14.25,22 14.46,21.82 14.5,21.58L14.87,18.93C15.5,18.67 16.04,18.34 16.56,17.94L19.05,18.95C19.27,19.03 19.54,18.95 19.66,18.73L21.66,15.27C21.78,15.05 21.73,14.78 21.54,14.63L19.43,12.97Z"),null),
                new SearchItem(2,"Item3","Warehouse item","Management","",Geometry.Parse("M12,15.5A3.5,3.5 0 0,1 8.5,12A3.5,3.5 0 0,1 12,8.5A3.5,3.5 0 0,1 15.5,12A3.5,3.5 0 0,1 12,15.5M19.43,12.97C19.47,12.65 19.5,12.33 19.5,12C19.5,11.67 19.47,11.34 19.43,11L21.54,9.37C21.73,9.22 21.78,8.95 21.66,8.73L19.66,5.27C19.54,5.05 19.27,4.96 19.05,5.05L16.56,6.05C16.04,5.66 15.5,5.32 14.87,5.07L14.5,2.42C14.46,2.18 14.25,2 14,2H10C9.75,2 9.54,2.18 9.5,2.42L9.13,5.07C8.5,5.32 7.96,5.66 7.44,6.05L4.95,5.05C4.73,4.96 4.46,5.05 4.34,5.27L2.34,8.73C2.21,8.95 2.27,9.22 2.46,9.37L4.57,11C4.53,11.34 4.5,11.67 4.5,12C4.5,12.33 4.53,12.65 4.57,12.97L2.46,14.63C2.27,14.78 2.21,15.05 2.34,15.27L4.34,18.73C4.46,18.95 4.73,19.03 4.95,18.95L7.44,17.94C7.96,18.34 8.5,18.68 9.13,18.93L9.5,21.58C9.54,21.82 9.75,22 10,22H14C14.25,22 14.46,21.82 14.5,21.58L14.87,18.93C15.5,18.67 16.04,18.34 16.56,17.94L19.05,18.95C19.27,19.03 19.54,18.95 19.66,18.73L21.66,15.27C21.78,15.05 21.73,14.78 21.54,14.63L19.43,12.97Z"),null),
            };
            SearchResults.CollectionView.SortDescriptions.Add(new SortDescription(nameof(SearchItem.Title), ListSortDirection.Ascending));
        }

        private void SearchTextChanged(string value)
        {
            Search(value);
        }

        /// <inheritdoc />
        protected override void OnDefaultViewCreated(SearchView e)
        {
            base.OnDefaultViewCreated( e);
            e.SearchBox.PreviewKeyDown += (send, args) =>
            {
                if (args.Key == Key.Down)
                {
                    View.ResultBox.Focus();
                    Keyboard.Focus(View.ResultBox);
                }
            };
        }

        /// <inheritdoc />
        public void Focus()
        {
            View.SearchBox.Focus();
            Keyboard.Focus(View.SearchBox);
        }
        
        private void Search(string term)
        {
            if (string.IsNullOrEmpty(term))
                return;
            SelectedResult = null;
            ShowPopup = true;
            ExecuteOnUIContext(() =>
            {
                SearchResults.Clear();
            });
            if (_lastOperation != null && !_lastOperation.IsCompleted)
                _lastOperation?.Abort();
            _lastOperation = _applicationSearchService.Search(term, ItemFound,
                                                  res =>
                                                  {
                                                      SearchResultsFound = res != null && res.Any();
                                                  });
        }

        private void AttachNavigationHandler(IEnumerable<SearchItem> items)
        {
            foreach (var searchItem in items)
            {
                searchItem.OnNavigate += Item_NaviagtionRequested;
            }
        }

        private void ItemFound(SearchItem obj)
        {
            ExecuteOnUIContext(() =>
            {
                SearchResults.Add(obj);
            });
        }

        /// <inheritdoc />
        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            ExecuteOnUIContext(() =>
            {
                SearchResults = AsyncObservableCollectionView<SearchItem>.Create((a, b) => true);
                SearchResults.CollectionView.SortDescriptions.Add(new SortDescription(nameof(SearchItem.Title), ListSortDirection.Ascending));
            });
            AttachNavigationHandler(_applicationSearchService.GetIndexedItems());
            _applicationSearchService.ItemsIndexed += (sender, args) => AttachNavigationHandler(args);
        }


        private void CloseSearch()
        {
            SearchText = "";
            View.SearchBox.Text = "";
            ShowPopup = false;
        }
        private void Item_NaviagtionRequested(object sender, SearchItem e)
        {
            CloseSearch();
            Keyboard.Focus(View);
            View.Focus();
        }
    }
}