using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coddee.Collections;
using Coddee.Mvvm;
using Coddee.Xamarin.Common;
using Coddee.Xamarin.Events;
using Xamarin.Forms;

namespace Coddee.Xamarin.Services.Navigation
{
    public class NavigationService : ViewModelBase<NavigationServiceDefaultView>, INavigationService
    {
        private INavigationItem _currentNavligationItem;
        public event EventHandler<NavigationEventArgs> Navigated;
        public void Initialize(IEnumerable<INavigationItem> navigationItems)
        {
            NavigationItems = AsyncObservableCollection<INavigationItem>.Create();
            navigationItems.ForEach(AddNavigationItem);
            _eventDispatcher.GetEvent<ApplicationStartedEvent>().Subscribe(e =>
            {
                if (navigationItems.Any())
                {
                    SelectedNavigationItem = navigationItems.First();
                    SelectedNavigationItem.IsSelected = true;
                    if (SelectedNavigationItem is NavigationItem nav)
                        nav.IsFirstItem = true;
                    SelectedNavigationItem.Navigate();
                }
            });
            ApplicationName = _globalVariables.GetVariable<ApplicationNameGlobalVariable>().GetValue();
        }

        public void AddNavigationItem(INavigationItem navigationItem)
        {
            NavigationItems.Add(navigationItem);
            if (!navigationItem.DestinationResolved)
                navigationItem.SetDestination((IPresentable)_vmManager.CreateViewModel(navigationItem.DestinationType, null));
            navigationItem.NavigationRequested += NavigationItem_NavigationRequested;
            navigationItem.Initialize();
        }
        private AsyncObservableCollection<INavigationItem> _navigationItems;
        public AsyncObservableCollection<INavigationItem> NavigationItems
        {
            get { return _navigationItems; }
            set { SetProperty(ref this._navigationItems, value); }
        }
        private INavigationItem _selectedNavigationItem;
        public INavigationItem SelectedNavigationItem
        {
            get { return _selectedNavigationItem; }
            set
            {
                SetProperty(ref this._selectedNavigationItem, value);
                value?.Navigate();
            }
        }
        private string _applicationName;
        public string ApplicationName
        {
            get { return _applicationName; }
            set { SetProperty(ref this._applicationName, value); }
        }
        private void NavigationItem_NavigationRequested(object sender, IPresentable e)
        {
            var nav = sender as INavigationItem;
            foreach (var navigationItem in NavigationItems)
            {
                navigationItem.IsSelected = false;
            }

            Navigated?.Invoke(this, new NavigationEventArgs {OldLocation = _currentNavligationItem, NewLocation = nav});
            _currentNavligationItem = nav;
            nav.IsSelected = true;
            
            var page = (Page)e.GetView();
            page.Title = ((NavigationItem) nav).Title;
            View.Detail=new NavigationPage(page);
            View.IsPresented = false;
        }
    }
}
