// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coddee.Mvvm
{
    /// <summary>
    /// Base class for ViewModel event that invokes the handlers in all the ViewModels in the same group.
    /// </summary>
    public class ViewModelsGroupEvent<TPayload> : IEvent
    {
        /// <inheritdoc />
        public ViewModelsGroupEvent()
        {
            _handlers = new Dictionary<string, List<ViewModelEventHandler<TPayload>>>();
            _asyncHandlers = new Dictionary<string, List<AsyncViewModelEventHandler<TPayload>>>();
        }

        /// <summary>
        /// The handlers subscribed to the event.
        /// </summary>
        protected readonly Dictionary<string, List<ViewModelEventHandler<TPayload>>> _handlers;

        /// <summary>
        /// The async handlers subscribed to the event.
        /// </summary>
        protected readonly Dictionary<string, List<AsyncViewModelEventHandler<TPayload>>> _asyncHandlers;

        /// <summary>
        /// Raise the event.
        /// </summary>
        /// <param name="group">The target ViewModels group</param>
        /// <param name="sender">The sender ViewModel</param>
        /// <param name="payload">The event arguments.</param>
        public virtual void Raise(string group, IViewModel sender, TPayload payload)
        {
            lock (_handlers)
            {
                if (_handlers.ContainsKey(group))
                    foreach (var handler in _handlers[group])
                    {
                        handler?.Invoke(sender, payload);
                    }
            }
        }

        /// <summary>
        /// Raise the event.
        /// </summary>
        /// <param name="group">The target ViewModels group</param>
        /// <param name="sender">The sender ViewModel</param>
        /// <param name="payload">The event arguments.</param>
        public virtual async Task RaiseAsync(string group, IViewModel sender, TPayload payload)
        {
            Raise(group, sender, payload);
            if (_asyncHandlers.ContainsKey(group))
                foreach (var handler in _asyncHandlers[group])
                {
                    await handler?.Invoke(sender, payload);
                }


        }

        /// <summary>
        /// Add a handler to the event.
        /// </summary>
        /// <param name="group">The ViewModel group.</param>
        /// <param name="handler">The event handler.</param>
        public virtual void Subscribe(string group, ViewModelEventHandler<TPayload> handler)
        {
            lock (_handlers)
            {
                var list = _handlers.ContainsKey(group) ? _handlers[group] : (_handlers[group] = new List<ViewModelEventHandler<TPayload>>());
                list.Add(handler);
            }
        }

        /// <summary>
        /// Remove a handler from the event.
        /// </summary>
        /// <param name="group">The ViewModel group.</param>
        /// <param name="handler">The event handler.</param>
        public virtual void Unsubscribe(string group, ViewModelEventHandler<TPayload> handler)
        {
            lock (_handlers)
            {
                if (_handlers.ContainsKey(group))
                    _handlers[group].Remove(handler);
            }
        }

        /// <summary>
        /// Add a handler to the event.
        /// </summary>
        /// <param name="group">The ViewModel group.</param>
        /// <param name="handler">The event handler.</param>
        public virtual void SubscribeAsync(string group, AsyncViewModelEventHandler<TPayload> handler)
        {
            lock (_asyncHandlers)
            {
                var list = _asyncHandlers.ContainsKey(group) ? _asyncHandlers[group] : (_asyncHandlers[group] = new List<AsyncViewModelEventHandler<TPayload>>());
                list.Add(handler);
            }
        }

        /// <summary>
        /// Remove a handler from the event.
        /// </summary>
        /// <param name="group">The ViewModel group.</param>
        /// <param name="handler">The event handler.</param>
        public virtual void UnsubscribeAsync(string group, AsyncViewModelEventHandler<TPayload> handler)
        {
            lock (_asyncHandlers)
            {
                if (_asyncHandlers.ContainsKey(group))
                    _asyncHandlers[group].Remove(handler);
            }
        }

        /// <inheritdoc />
        public EventRoutingStrategy EventRoutingStrategy => EventRoutingStrategy.Direct;
    }
}
