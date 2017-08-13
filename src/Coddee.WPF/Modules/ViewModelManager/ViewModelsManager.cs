// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
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
            ViewModels = new Dictionary<IViewModel, ViewModelInfo>();
        }

        public event EventHandler<ViewModelInfo> ViewModelCreated;

        public Dictionary<IViewModel, ViewModelInfo> ViewModels { get; }

        public ViewModelInfo RootViewModel { get; protected set; }

        public IViewModel CreateViewModel(Type viewModelType, IViewModel parentVM)
        {
            lock (ViewModels)
            {
                IViewModel vm = (IViewModel)_container.Resolve(viewModelType);
                var id = ++_lastID;
                _logger?.Log(_eventsSource, $"ViewModelCreated {viewModelType.Name}", LogRecordTypes.Debug);
                var vmInfo = new ViewModelInfo(vm) { ID = id };

                if (!ViewModels.Any() && parentVM == null)
                {
                    RootViewModel = vmInfo;
                }
                else if (parentVM == null && RootViewModel != null)
                    parentVM = RootViewModel.ViewModel;
                
                if (parentVM!=null && ViewModels.ContainsKey(parentVM))
                {
                    var parent = ViewModels[parentVM];
                    vmInfo.ParentViewModel = parent;
                    parent.ChildViewModels.Add(vmInfo);
                }

                ViewModels.Add(vm, vmInfo);
                ViewModelCreated?.Invoke(this, vmInfo);

                return vm;
            }
        }

        public TResult CreateViewModel<TResult>(IViewModel parentVM) where TResult : IViewModel
        {
            return (TResult)CreateViewModel(typeof(TResult), parentVM);
        }

        public async Task<IViewModel> InitializeViewModel(Type viewModelType, IViewModel parentVM)
        {
            var vm = CreateViewModel(viewModelType, parentVM);
            await vm.Initialize();
            return vm;
        }

        public IEnumerable<ViewModelInfo> GetChildViewModels(IViewModel parent)
        {
            return ViewModels[parent].ChildViewModels;
        }

        public async Task<TResult> InitializeViewModel<TResult>(IViewModel parentVM) where TResult : IViewModel
        {
            return (TResult)await InitializeViewModel(typeof(TResult), parentVM);
        }
    }
}
