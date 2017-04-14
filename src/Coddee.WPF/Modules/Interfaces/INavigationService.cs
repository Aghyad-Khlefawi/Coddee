// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using Coddee.WPF.Navigation;

namespace Coddee.WPF.Modules
{
    public interface INavigationService
    {
        void Initialize(Region navbarRegion, Region navigationRegion, IEnumerable<INavigationItem> navigationItems);
        void AddNavigationItem(INavigationItem navigationItem);
    }
}