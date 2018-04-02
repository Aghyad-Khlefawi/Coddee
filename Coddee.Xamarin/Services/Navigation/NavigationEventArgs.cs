using System;
using System.Collections.Generic;
using System.Text;

namespace Coddee.Xamarin.Services.Navigation
{
    public class NavigationEventArgs
    {
        public INavigationItem OldLocation { get; set; }
        public INavigationItem NewLocation { get; set; }
    }
}