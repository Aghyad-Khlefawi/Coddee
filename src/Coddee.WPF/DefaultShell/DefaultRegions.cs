// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.WPF.DefaultShell
{
    public static class DefaultRegions
    {
        public static Region ApplicationMainRegion { get; } = Region.CreateRegion(nameof(ApplicationMainRegion));
        public static Region ToastRegion { get; } = Region.CreateRegion(nameof(ToastRegion));
        public static Region NavbarRegion { get; } = Region.CreateRegion(nameof(NavbarRegion));
        public static Region DialogRegion { get; } = Region.CreateRegion(nameof(DialogRegion));
        public static Region NotificationRegion { get; } = Region.CreateRegion(nameof(NotificationRegion));
    }
}