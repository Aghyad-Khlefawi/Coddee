// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;

namespace Coddee.Mvvm
{
    /// <summary>
    /// Base class for ViewModel events.
    /// </summary>
    public abstract class ViewModelEvent
    {
        /// <summary>
        /// <see cref="IViewModelsManager"/> instance used by the application.
        /// </summary>
        protected static IViewModelsManager _viewModelsManager;

        /// <summary>
        /// set the instance of <see cref="IViewModelsManager"/> used by the application.
        /// </summary>
        /// <param name="viewModelsManager"></param>
        public static void SetViewModelManager(IViewModelsManager viewModelsManager)
        {
            _viewModelsManager = viewModelsManager;
        }
    }

    /// <summary>
    /// Base class for ViewModel events.
    /// </summary>
    public abstract class ViewModelEvent<TPayload> : ViewModelEvent, IViewModelEvent<TPayload>
    {
        /// <inheritdoc />
        protected ViewModelEvent(EventRoutingStrategy routingStrategy)
        {
            _handlers = new Dictionary<IViewModel, ViewModelEventHandler<TPayload>>();
            EventRoutingStrategy = routingStrategy;
        }

        /// <summary>
        /// The handlers subscribed to the event.
        /// </summary>
        protected readonly Dictionary<IViewModel, ViewModelEventHandler<TPayload>> _handlers;

        /// <inheritdoc />
        public EventRoutingStrategy EventRoutingStrategy { get; protected set; }

        /// <summary>
        /// Returns a handler by ViewModel.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public ViewModelEventHandler<TPayload> GetHandler(IViewModel viewModel)
        {
            if (_handlers.ContainsKey(viewModel))
                return _handlers[viewModel];
            return null;
        }

        /// <inheritdoc />
        public abstract void Raise(IViewModel sender, TPayload args);

        /// <inheritdoc />
        public virtual void Subscribe(IViewModel viewModel, ViewModelEventHandler<TPayload> handler)
        {
            _handlers.Add(viewModel, handler);
        }

        /// <inheritdoc />
        public virtual void Unsubscribe(IViewModel viewModel)
        {
            _handlers.Remove(viewModel);
        }
    }
}
