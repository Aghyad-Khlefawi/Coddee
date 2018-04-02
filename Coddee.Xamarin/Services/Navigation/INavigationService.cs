using System;
using System.Collections.Generic;

namespace Coddee.Xamarin.Services.Navigation
{
    public interface INavigationService
    {
        event EventHandler<NavigationEventArgs> Navigated;
        void Initialize(IEnumerable<INavigationItem> navigationItems);
        void AddNavigationItem(INavigationItem navigationItem);
    }
}
