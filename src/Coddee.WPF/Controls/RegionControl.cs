// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;
using System.Windows.Controls;

namespace Coddee.WPF.Controls
{
    public class RegionControl:ContentControl
    {
        static RegionControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RegionControl), new FrameworkPropertyMetadata(typeof(RegionControl)));
        }

        public static readonly DependencyProperty RegionProperty = DependencyProperty.Register(
            "Region", typeof(Region), typeof(RegionControl), new PropertyMetadata(default(Region)));

        public Region Region
        {
            get { return (Region) GetValue(RegionProperty); }
            set { SetValue(RegionProperty, value); }
        }
    }
}
