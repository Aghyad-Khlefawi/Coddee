// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coddee.Mvvm
{
    /// <summary>
    /// A service that manages the creation and disposal of <see cref="IViewModel"/> objects.
    /// </summary>
    public interface IViewModelsManager
    {
        /// <summary>
        /// The main ViewModel of the application.
        /// </summary>
        ViewModelInfo RootViewModel { get; }


        /// <summary>
        /// Triggered when a new ViewModel is created.
        /// </summary>
        event EventHandler<ViewModelInfo> ViewModelCreated;

        /// <summary>
        /// Create a new ViewModel instance
        /// </summary>
        /// <param name="viewModelType">The type of the ViewModel to be created.</param>
        /// <param name="parentVM">The ViewModel that is creating the new ViewModel.</param>
        IViewModel CreateViewModel(Type viewModelType, IViewModel parentVM);

        /// <summary>
        /// Create a new ViewModel instance
        /// </summary>
        /// <param name="viewModelType">The type of the ViewModel to be created.</param>
        /// <param name="parentVM">The ViewModel that is creating the new ViewModel.</param>
        /// <param name="options">The ViewModel options object.</param>
        IViewModel CreateViewModel(Type viewModelType, IViewModel parentVM, ViewModelOptions options);

        /// <summary>
        /// Create a new ViewModel instance
        /// </summary>
        /// <typeparam name="TResult">The type of the ViewModel to be created.</typeparam>
        /// <param name="parentVM">The ViewModel that is creating the new ViewModel.</param>
        TResult CreateViewModel<TResult>(IViewModel parentVM) where TResult : IViewModel;

        /// <summary>
        /// Create a new ViewModel instance
        /// </summary>
        /// <typeparam name="TResult">The type of the ViewModel to be created.</typeparam>
        /// <param name="options">The ViewModel options object.</param>
        /// <param name="parentVM">The ViewModel that is creating the new ViewModel.</param>
        TResult CreateViewModel<TResult>(IViewModel parentVM, ViewModelOptions options) where TResult : IViewModel;


        /// <summary>
        /// Create a new ViewModel instance an initializes it
        /// </summary>
        /// <param name="viewModelType">The type of the ViewModel to be created.</param>
        /// <param name="parentVM">The ViewModel that is creating the new ViewModel.</param>
        Task<IViewModel> InitializeViewModel(Type viewModelType, IViewModel parentVM);

        /// <summary>
        /// Create a new ViewModel instance an initializes it
        /// </summary>
        /// <typeparam name="TResult">The type of the ViewModel to be created.</typeparam>
        /// <param name="parentVM">The ViewModel that is creating the new ViewModel.</param>
        Task<TResult> InitializeViewModel<TResult>(IViewModel parentVM) where TResult:IViewModel;

        /// <summary>
        /// Returns the ViewModels created by a specific ViewModel.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ViewModelInfo> GetChildViewModels(IViewModel parent);

        /// <summary>
        /// Returns the information object of a ViewModel parent.
        /// </summary>
        /// <returns></returns>
        ViewModelInfo GetParentViewModel(IViewModel viewModel);

        /// <summary>
        /// Sets a <see cref="IViewModel.ViewModelGroup"/> in a group.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="viewModel"></param>
        void AddViewModelToGroup(string group, IViewModel viewModel);


        /// <summary>
        /// Returns all the ViewModels that are in a specific group.
        /// </summary>
        IEnumerable<IViewModel> GetGroupViewModels(string group);

        /// <summary>
        /// Dispose of a ViewModel.
        /// </summary>
        void RemoveViewModel(IViewModel viewModel);
    }
}