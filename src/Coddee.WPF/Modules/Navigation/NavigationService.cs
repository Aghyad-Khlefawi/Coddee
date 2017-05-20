// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Coddee.Collections;
using Coddee.WPF.Commands;
using Coddee.WPF.Navigation;

namespace Coddee.WPF.Modules.Navigation
{
    public class NavigationService : ViewModelBase<NavigationServiceView>, INavigationService
    {
        public NavigationService()
        {
            if (IsDesignMode())
            {
                NavigationItems = new AsyncObservableCollection<INavigationItem>
                {
                    new NavigationItem<NavigationService>("Home", "M10,20V14H14V20H19V12H22L12,3L2,12H5V20H10Z")
                    {
                        IsSelected = true,
                        IsFirstItem = true
                    },
                    new NavigationItem<NavigationService>(
                                                          "Settings",
                                                          "M12,15.5A3.5,3.5 0 0,1 8.5,12A3.5,3.5 0 0,1 12,8.5A3.5,3.5 0 0,1 15.5,12A3.5,3.5 0 0,1 12,15.5M19.43,12.97C19.47,12.65 19.5,12.33 19.5,12C19.5,11.67 19.47,11.34 19.43,11L21.54,9.37C21.73,9.22 21.78,8.95 21.66,8.73L19.66,5.27C19.54,5.05 19.27,4.96 19.05,5.05L16.56,6.05C16.04,5.66 15.5,5.32 14.87,5.07L14.5,2.42C14.46,2.18 14.25,2 14,2H10C9.75,2 9.54,2.18 9.5,2.42L9.13,5.07C8.5,5.32 7.96,5.66 7.44,6.05L4.95,5.05C4.73,4.96 4.46,5.05 4.34,5.27L2.34,8.73C2.21,8.95 2.27,9.22 2.46,9.37L4.57,11C4.53,11.34 4.5,11.67 4.5,12C4.5,12.33 4.53,12.65 4.57,12.97L2.46,14.63C2.27,14.78 2.21,15.05 2.34,15.27L4.34,18.73C4.46,18.95 4.73,19.03 4.95,18.95L7.44,17.94C7.96,18.34 8.5,18.68 9.13,18.93L9.5,21.58C9.54,21.82 9.75,22 10,22H14C14.25,22 14.46,21.82 14.5,21.58L14.87,18.93C15.5,18.67 16.04,18.34 16.56,17.94L19.05,18.95C19.27,19.03 19.54,18.95 19.66,18.73L21.66,15.27C21.78,15.05 21.73,14.78 21.54,14.63L19.43,12.97Z",
                                                          NavItemPosition.Bottom)
                    {
                        IsSelected = false
                    }
                };
            }
        }
        

        private bool _showTitles;
        public bool ShowTitles
        {
            get { return _showTitles; }
            set
            {
                SetProperty(ref this._showTitles, value);
                NavigationItems?.ForEach(e => e.ShowTitle = value);
            }
        }

        private Region _navigationRegion;

        public void Initialize(Region navbarRegion,
                               Region navigationRegion,
                               IEnumerable<INavigationItem> navigationItems)
        {
            _navigationRegion = navigationRegion;
            NavigationItems = AsyncObservableCollection<INavigationItem>.Create();
            navigationItems.ForEach(AddNavigationItem);
            navbarRegion.View(this);
        }

        public void AddNavigationItem(INavigationItem navigationItem)
        {
            NavigationItems.Add(navigationItem);
            if (!navigationItem.DestinationResolved)
                navigationItem.SetDestination((IPresentable) Resolve<IShellViewModel>()
                                                  .CreateViewModel(navigationItem.DestinationType));
            navigationItem.NavigationRequested += NavigationItem_NavigationRequested;
            navigationItem.Initialize();
        }

        private AsyncObservableCollection<INavigationItem> _navigationItems;
        public AsyncObservableCollection<INavigationItem> NavigationItems
        {
            get { return _navigationItems; }
            set { SetProperty(ref this._navigationItems, value); }
        }
        public ICommand ExpandMenuCommand => new RelayCommand(ExpandMenu);

        private void ExpandMenu()
        {
            ShowTitles = !ShowTitles;
        }

        private void NavigationItem_NavigationRequested(object sender, IPresentable e)
        {
            var nav = sender as INavigationItem;
            foreach (var navigationItem in NavigationItems)
            {
                navigationItem.IsSelected = false;
            }
            _navigationRegion.View(e);
            ShowTitles = false;
            nav.IsSelected = true;
        }
    }
}