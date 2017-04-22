﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;

namespace Coddee.WPF
{
    /// <summary>
    /// Implemented by object that have an associated view
    /// </summary>
    public interface IPresentable<TView> where TView:UIElement
    {
        /// <summary>
        /// Returns the view associated with this object
        /// </summary>
        /// <returns></returns>
        TView GetView();
    }
    /// <summary>
    /// Implemented by object that have an associated view
    /// </summary>
    public interface IPresentable:IPresentable<UIElement>
    {

    }
}
