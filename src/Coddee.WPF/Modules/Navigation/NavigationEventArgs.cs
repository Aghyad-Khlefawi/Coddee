// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Services.Navigation
{
   public class NavigationEventArgs
    {
        public INavigationItem OldLocation { get; set; }
        public INavigationItem NewLocation { get; set; }
    }
}
