// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Windows;

namespace Coddee.WPF
{
    /// <summary>
    /// This interface represent a WPF application shell ViewModel
    /// </summary>
    public interface IShellViewModel:IPresentableViewModel
    {

    }

    /// <summary>
    /// This interface represent a WPF application shell ViewModel
    /// </summary>
    public interface IDefaultShellViewModel : IShellViewModel
    {
        IPresentableViewModel SetMainContent(Type defaultPresentable, bool useNavigation);
        IPresentableViewModel GetMainContent();

        void SetToolbarContent(UIElement content);
    }
}