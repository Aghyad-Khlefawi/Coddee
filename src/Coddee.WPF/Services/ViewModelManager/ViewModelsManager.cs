// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coddee.Loggers;
using Coddee.WPF;
using Coddee.WPF.Events;


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
            _events = new Dictionary<Type, IViewModelEvent>();
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

                if (parentVM != null && ViewModels.ContainsKey(parentVM))
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
            if (ViewModels.ContainsKey(parent))
                return ViewModels[parent].ChildViewModels;
            return new List<ViewModelInfo>();
        }

        public async Task<TResult> InitializeViewModel<TResult>(IViewModel parentVM) where TResult : IViewModel
        {
            return (TResult)await InitializeViewModel(typeof(TResult), parentVM);
        }

        public void RaiseEvent<TEvent, TArgs>(IViewModel sender, TEvent eventToRaise, TArgs args) where TEvent : IViewModelEvent<TArgs>, new()
        {
            if (eventToRaise.EventRoutingStrategy == EventRoutingStrategy.Tunnel)
            {
                InvokeTunnelEvent(sender, eventToRaise, args, GetChildViewModels(sender));
            }
            else if (eventToRaise.EventRoutingStrategy == EventRoutingStrategy.Bubble)
            {
                InvokeBubbleEvent(sender, eventToRaise, args, ViewModels[sender].ParentViewModel.ViewModel);
            }
        }

        private void InvokeBubbleEvent<TEvent, TArgs>(IViewModel sender, TEvent eventToRaise, TArgs args, IViewModel parentViewModel) where TEvent : IViewModelEvent<TArgs>, new()
        {

            var handler = eventToRaise.GetHandler(parentViewModel);
            handler?.Invoke(sender, args);
            if (ViewModels[parentViewModel].ParentViewModel != null)
                InvokeBubbleEvent(sender, eventToRaise, args, ViewModels[parentViewModel].ParentViewModel.ViewModel);
        }

        private void InvokeTunnelEvent<TEvent, TArgs>(IViewModel sender, TEvent eventToRaise, TArgs args, IEnumerable<ViewModelInfo> childViewModels) where TEvent : IViewModelEvent<TArgs>, new()
        {
            foreach (var childViewModel in childViewModels)
            {
                var handler = eventToRaise.GetHandler(childViewModel.ViewModel);
                handler?.Invoke(sender, args);
                InvokeTunnelEvent(sender, eventToRaise, args, GetChildViewModels(childViewModel.ViewModel));
            }
        }

        private readonly Dictionary<Type, IViewModelEvent> _events;

        public TEvent GetEvent<TEvent>() where TEvent : IViewModelEvent, new()
        {
            if (!_events.ContainsKey(typeof(TEvent)))
                _events[typeof(TEvent)] = new TEvent();

            return (TEvent)_events[typeof(TEvent)];
        }

        public void SubscribeToEvent<TEvent, TArgs>(IViewModel subscriver, ViewModelEventHandler<TArgs> handler)
            where TEvent : IViewModelEvent<TArgs>, new()
        {
            var eventToSubscribe = GetEvent<TEvent>();
            eventToSubscribe.Subscribe(subscriver, handler);
        }
    }
}
