using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coddee.Services.ViewModelManager;
using Coddee.WPF;

namespace Coddee.Services
{
    public interface IViewModelsManager
    {
        ViewModelInfo RootViewModel { get; }
        Dictionary<IViewModel, ViewModelInfo> ViewModels { get; }

        event EventHandler<ViewModelInfo> ViewModelCreated;

        IViewModel CreateViewModel(Type viewModelType, IViewModel parentVM);
        TResult CreateViewModel<TResult>(IViewModel parentVM) where TResult : IViewModel;

        Task<IViewModel> InitializeViewModel(Type viewModelType, IViewModel parentVM);
        Task<TResult> InitializeViewModel<TResult>(IViewModel parentVM) where TResult:IViewModel;


        IEnumerable<ViewModelInfo> GetChildViewModels(IViewModel parent);
    }
}