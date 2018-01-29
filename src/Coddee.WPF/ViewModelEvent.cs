// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using Coddee.Services;

namespace Coddee.WPF
{
    public interface IViewModelEvent<TPayload> : IEvent
    {
        void Raise(IViewModel sender, TPayload args);
        void Subscribe(IViewModel viewModel, ViewModelEventHandler<TPayload> handler);
        void Unsubscribe(IViewModel viewModel);
    }

    public abstract class ViewModelEvent<TPayload> : IViewModelEvent<TPayload>
    {
        protected ViewModelEvent(EventRoutingStrategy routingStrategy)
        {
            _handlers = new Dictionary<IViewModel, ViewModelEventHandler<TPayload>>();
            EventRoutingStrategy = routingStrategy;
            _viewModelsManager = WPFApplication.Current.GetContainer().Resolve<IViewModelsManager>();
        }

        protected readonly IViewModelsManager _viewModelsManager;

        protected readonly Dictionary<IViewModel, ViewModelEventHandler<TPayload>> _handlers;

        public EventRoutingStrategy EventRoutingStrategy { get; protected set; }

        public ViewModelEventHandler<TPayload> GetHandler(IViewModel viewModel)
        {
            if (_handlers.ContainsKey(viewModel))
                return _handlers[viewModel];
            return null;
        }

        public abstract void Raise(IViewModel sender, TPayload args);

        public virtual void Subscribe(IViewModel viewModel, ViewModelEventHandler<TPayload> handler)
        {
            _handlers.Add(viewModel, handler);
        }

        public virtual void Unsubscribe(IViewModel viewModel)
        {
            _handlers.Remove(viewModel);
        }
    }

    public class ViewModelTunnelEvent<TPayload> : ViewModelEvent<TPayload>
    {
        public ViewModelTunnelEvent() : base(EventRoutingStrategy.Tunnel)
        {

        }

        public override void Raise(IViewModel sender, TPayload args)
        {
            InvokeChildren(sender, sender, args);
        }

        private void InvokeChildren(IViewModel sender, IViewModel target, TPayload args)
        {
            var childViewModels = _viewModelsManager.GetChildViewModels(target);
            foreach (var childViewModel in childViewModels)
            {
                var handler = GetHandler(childViewModel.ViewModel);
                handler?.Invoke(sender, args);
                InvokeChildren(sender, childViewModel.ViewModel, args);
            }
        }
    }

    public class ViewModelBubbleEvent<TPayload> : ViewModelEvent<TPayload>
    {
        public ViewModelBubbleEvent() : base(EventRoutingStrategy.Bubble)
        {

        }

        public override void Raise(IViewModel sender, TPayload args)
        {
            InvokeParent(sender, sender, args);
        }

        private void InvokeParent(IViewModel sender, IViewModel target, TPayload args)
        {
            var parentViewModels = _viewModelsManager.GetParentViewModel(target);
            if (parentViewModels != null)
            {
                var handler = GetHandler(parentViewModels.ViewModel);
                handler?.Invoke(sender, args);
                InvokeParent(sender, parentViewModels.ViewModel, args);
            }
        }
    }
}
