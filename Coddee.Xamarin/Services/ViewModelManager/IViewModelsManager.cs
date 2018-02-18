using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coddee.Xamarin.Common;

namespace Coddee.Xamarin.Services.ViewModelManager
{
    public interface IViewModelsManager
    {
        ViewModelInfo RootViewModel { get; }

        event EventHandler<ViewModelInfo> ViewModelCreated;

        IViewModel CreateViewModel(Type viewModelType, IViewModel parentVM);
        IViewModel CreateViewModel(Type viewModelType, IViewModel parentVM, ViewModelOptions options);
        TResult CreateViewModel<TResult>(IViewModel parentVM) where TResult : IViewModel;
        TResult CreateViewModel<TResult>(IViewModel parentVM, ViewModelOptions options) where TResult : IViewModel;

        Task<IViewModel> InitializeViewModel(Type viewModelType, IViewModel parentVM);
        Task<TResult> InitializeViewModel<TResult>(IViewModel parentVM) where TResult : IViewModel;


        IEnumerable<ViewModelInfo> GetChildViewModels(IViewModel parent);
        ViewModelInfo GetParentViewModel(IViewModel viewModel);

        void AddViewModelToGroup(string group, IViewModel viewModel);
        IEnumerable<IViewModel> GetGroupViewModels(string group);
        void RemoveViewModel(IViewModel viewModel);
    }
}