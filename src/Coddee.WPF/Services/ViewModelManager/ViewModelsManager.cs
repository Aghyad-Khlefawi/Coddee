// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Coddee.Loggers;
using Coddee.WPF;


namespace Coddee.Services.ViewModelManager
{

    public class ViewModelsManager : IViewModelsManager
    {
        private const string _eventsSource = "ViewModelsManager";

        private readonly IContainer _container;
        private readonly ILogger _logger;
        private int _lastID;

        public ViewModelsManager(IContainer container, ILogger logger)
        {
            _container = container;
            _logger = logger;
            _viewModels = new ConcurrentDictionary<IViewModel, ViewModelInfo>();
            _viewModelGroups = new ConcurrentDictionary<string, List<IViewModel>>();
        }


        public event EventHandler<ViewModelInfo> ViewModelCreated;

        protected ConcurrentDictionary<IViewModel, ViewModelInfo> _viewModels;
        protected ConcurrentDictionary<string, List<IViewModel>> _viewModelGroups;

        public ViewModelInfo RootViewModel { get; protected set; }

        public void AddViewModelToGroup(string group, IViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.ViewModelGroup) && _viewModelGroups.ContainsKey(viewModel.ViewModelGroup))
                _viewModelGroups[viewModel.ViewModelGroup].Remove(viewModel);

            var list = _viewModelGroups.ContainsKey(group) ? _viewModelGroups[group] : (_viewModelGroups[group] = new List<IViewModel>());
            list.Add(viewModel);
        }

        public IEnumerable<IViewModel> GetGroupViewModels(string group)
        {
            return _viewModelGroups[group];
        }

        public void RemoveViewModel(IViewModel viewModel)
        {
            lock (_viewModels)
            {
                if (_viewModels.ContainsKey(viewModel))
                {
                    _viewModels.TryRemove(viewModel, out ViewModelInfo deleted);
                    deleted?.ParentViewModel?.ChildViewModels.Remove(deleted);
                }
            }
        }

        public IViewModel CreateViewModel(Type viewModelType, IViewModel parentVM)
        {
            return CreateViewModel(viewModelType, parentVM, null);
        }

        public IViewModel CreateViewModel(Type viewModelType, IViewModel parentVM, ViewModelOptions viewModelOptions)
        {
            lock (_viewModels)
            {
                IViewModel vm = (IViewModel)_container.Resolve(viewModelType);

                if (viewModelOptions == null)
                {
                    viewModelOptions = vm.DefaultViewModelOptions;
                }
                vm.SetViewModelOptions(viewModelOptions);
                var id = ++_lastID;
                _logger?.Log(_eventsSource, $"ViewModelCreated {viewModelType.Name}", LogRecordTypes.Debug);
                var vmInfo = new ViewModelInfo(vm) { ID = id };

                if (!_viewModels.Any() && parentVM == null)
                {
                    RootViewModel = vmInfo;
                }
                else if (parentVM == null && RootViewModel != null)
                    parentVM = RootViewModel.ViewModel;

                if (parentVM != null && _viewModels.ContainsKey(parentVM))
                {
                    var parent = _viewModels[parentVM];
                    vmInfo.ParentViewModel = parent;
                    parent.ChildViewModels.Add(vmInfo);
                }

                _viewModels.TryAdd(vm, vmInfo);
                ViewModelCreated?.Invoke(this, vmInfo);

                return vm;
            }
        }

        public TResult CreateViewModel<TResult>(IViewModel parentVM) where TResult : IViewModel
        {
            return (TResult)CreateViewModel(typeof(TResult), parentVM);
        }

        public TResult CreateViewModel<TResult>(IViewModel parentVM, ViewModelOptions viewModelOptions) where TResult : IViewModel
        {
            return (TResult)CreateViewModel(typeof(TResult), parentVM, viewModelOptions);
        }

        public async Task<IViewModel> InitializeViewModel(Type viewModelType, IViewModel parentVM)
        {
            var vm = CreateViewModel(viewModelType, parentVM);
            await vm.Initialize();
            return vm;
        }

        public IEnumerable<ViewModelInfo> GetChildViewModels(IViewModel parent)
        {
            if (_viewModels.ContainsKey(parent))
                return _viewModels[parent].ChildViewModels;
            return new List<ViewModelInfo>();
        }
        public ViewModelInfo GetParentViewModel(IViewModel viewModel)
        {
            if (_viewModels.ContainsKey(viewModel))
                return _viewModels[viewModel].ParentViewModel;
            return null;
        }

        public async Task<TResult> InitializeViewModel<TResult>(IViewModel parentVM) where TResult : IViewModel
        {
            return (TResult)await InitializeViewModel(typeof(TResult), parentVM);
        }
    }
}
