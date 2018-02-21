// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Windows;
using Coddee.Mvvm;

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
        /// <summary>
        /// Set the main content of the application.
        /// </summary>
        /// <returns></returns>
        IPresentableViewModel SetMainContent(Type defaultPresentable, bool useNavigation);

        /// <summary>
        /// returns the main content of the application.
        /// </summary>
        /// <returns></returns>
        IPresentableViewModel GetMainContent();


        /// <summary>
        /// Sets the tool-bar content.
        /// </summary>
        /// <param name="content"></param>
        void SetToolbarContent(UIElement content);
    }
}