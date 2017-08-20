// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using Coddee.WPF;
using Coddee.Services.Navigation;

namespace Coddee.Services
{
    public interface INavigationService
    {
        event EventHandler<NavigationEventArgs> Navigated;
        void Initialize(Region navbarRegion, Region navigationRegion, IEnumerable<INavigationItem> navigationItems);
        void AddNavigationItem(INavigationItem navigationItem);
    }
}