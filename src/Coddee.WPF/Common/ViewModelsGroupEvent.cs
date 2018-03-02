// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Threading.Tasks;
using Coddee.Mvvm;

namespace Coddee.WPF
{
    /// <inheritdoc />
    public class ViewModelsGroupEvent<TPayload> : IEvent
    {
        /// <inheritdoc />
        public ViewModelsGroupEvent()
        {
            _handlers = new Dictionary<string, List<ViewModelEventHandler<TPayload>>>();
            _asyncHandlers = new Dictionary<string, List<AsyncViewModelEventHandler<TPayload>>>();
        }

        /// <summary>
        /// Event handlers.
        /// </summary>
        protected readonly Dictionary<string, List<ViewModelEventHandler<TPayload>>> _handlers;
        
        /// <summary>
        /// Event async handlers.
        /// </summary>
        protected readonly Dictionary<string, List<AsyncViewModelEventHandler<TPayload>>> _asyncHandlers;

        /// <summary>
        /// Invoke the event handlers.
        /// </summary>
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
        /// Invoke the event async handlers.
        /// </summary>
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
        public virtual void Subscribe(string group, ViewModelEventHandler<TPayload> handler)
        {
            lock (_handlers)
            {
                var list = _handlers.ContainsKey(group) ? _handlers[group] : (_handlers[group] = new List<ViewModelEventHandler<TPayload>>());
                list.Add(handler);
            }
        }

        /// <summary>
        /// Unsubscribe a handler to the event.
        /// </summary>
        public virtual void Unsubscribe(string group, ViewModelEventHandler<TPayload> handler)
        {
            lock (_handlers)
            {
                if (_handlers.ContainsKey(group))
                    _handlers[group].Remove(handler);
            }
        }

        /// <summary>
        /// Add an async handler to the event.
        /// </summary>
        public virtual void SubscribeAsync(string group, AsyncViewModelEventHandler<TPayload> handler)
        {
            lock (_asyncHandlers)
            {
                var list = _asyncHandlers.ContainsKey(group) ? _asyncHandlers[group] : (_asyncHandlers[group] = new List<AsyncViewModelEventHandler<TPayload>>());
                list.Add(handler);
            }
        }

        /// <summary>
        /// Unsubscribe an async handler to the event.
        /// </summary>
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
