// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;

namespace Coddee.WPF
{
    /// <summary>
    /// This interface represent a WPF application shell ViewModel
    /// </summary>
    public interface IShellViewModel:IViewModel
    {
        IViewModel CreateViewModel(Type viewModelType);
        TViewModel CreateViewModel<TViewModel>() where TViewModel : IViewModel;
    }

    /// <summary>
    /// This interface represent a WPF application shell ViewModel
    /// </summary>
    public interface IDefaultShellViewModel : IShellViewModel
    {
        IViewModel SetMainContent(Type defaultPresentable, bool useNavigation);
    }
}