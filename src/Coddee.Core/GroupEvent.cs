// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coddee
{
    /// <summary>
    /// An event that when triggered only executed the handlers in the same group.
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class GroupEvent<TPayload> : IEvent
    {
        /// <inheritdoc />
        public GroupEvent()
        {
            _handlers = new Dictionary<string, List<Action<TPayload>>>();
            _asyncHandlers = new Dictionary<string, List<Func<TPayload, Task>>>();
        }

        /// <summary>
        /// The handlers of the event.
        /// </summary>
        protected readonly Dictionary<string, List<Action<TPayload>>> _handlers;


        /// <summary>
        /// The Async handlers of the event.
        /// </summary>
        protected readonly Dictionary<string, List<Func<TPayload, Task>>> _asyncHandlers;

        /// <summary>
        /// Invoke the event handlers.
        /// </summary>
        public virtual void Raise(string group, TPayload payload)
        {
            if (_handlers.ContainsKey(group))
                foreach (var handler in _handlers[group])
                {
                    handler?.Invoke(payload);
                }
        }

        /// <summary>
        /// Invoke the event Async handlers.
        /// </summary>
        public virtual async Task RaiseAsync(string group, TPayload payload)
        {
            Raise(group, payload);
            if (_asyncHandlers.ContainsKey(group))
                foreach (var handler in _asyncHandlers[group])
                {
                    await handler?.Invoke(payload);
                }
        }

        /// <summary>
        /// Add a handler to the event.
        /// </summary>
        public virtual void Subscribe(string group, Action<TPayload> handler)
        {
            var list = _handlers.ContainsKey(group) ? _handlers[group] : (_handlers[group] = new List<Action<TPayload>>());
            list.Add(handler);
        }

        /// <summary>
        /// Add an Async handler to the event.
        /// </summary>
        public virtual void SubscribeAsync(string group, Func<TPayload, Task> handler)
        {
            var list = _asyncHandlers.ContainsKey(group) ? _asyncHandlers[group] : (_asyncHandlers[group] = new List<Func<TPayload, Task>>());
            list.Add(handler);
        }

        /// <summary>
        /// Unsubscribe a handler to the event.
        /// </summary>
        public virtual void Unsubscribe(string group, Action<TPayload> handler)
        {
            if (_handlers.ContainsKey(group))
                _handlers[group].Remove(handler);
        }

        /// <summary>
        /// Unsubscribe an Async handler to the event.
        /// </summary>
        public virtual void UnsubscribeAsync(string group, Func<TPayload, Task> handler)
        {
            if (_asyncHandlers.ContainsKey(group))
                _asyncHandlers[group].Remove(handler);
        }

        /// <inheritdoc />
        public EventRoutingStrategy EventRoutingStrategy => EventRoutingStrategy.Direct;
    }
}
