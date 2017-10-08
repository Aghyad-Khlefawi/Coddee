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


        void RaiseEvent<TEvent, TArgs>(IViewModel sender, TEvent eventToRaise, TArgs args)
            where TEvent : IViewModelEvent<TArgs>, new();

        TEvent GetEvent<TEvent>() where TEvent : IViewModelEvent, new();

        void SubscribeToEvent<TEvent, TArgs>(IViewModel subscriver, ViewModelEventHandler<TArgs> handler)
            where TEvent : IViewModelEvent<TArgs>, new();
    }
}