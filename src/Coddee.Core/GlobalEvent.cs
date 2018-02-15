// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;

namespace Coddee
{
    /// <summary>
    /// An event that can be raised or subscribed to from anywhere in the application
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class GlobalEvent<TPayload> : IEvent
    {
        /// <inheritdoc />
        public GlobalEvent()
        {
            _handlers = new List<Action<TPayload>>();
        }

        /// <summary>
        /// The handlers of the event.
        /// </summary>
        protected readonly List<Action<TPayload>> _handlers;

        /// <summary>
        /// Invoke the event handlers.
        /// </summary>
        public virtual void Raise(TPayload payload)
        {
            foreach (var handler in _handlers)
            {
                handler?.Invoke(payload);
            }
        }

        /// <summary>
        /// Add a handler to the event.
        /// </summary>
        public virtual void Subscribe(Action<TPayload> handler)
        {
            _handlers.Add(handler);
        }

        /// <summary>
        /// Unsubscribe a handler to the event.
        /// </summary>
        public virtual void Unsubscribe(Action<TPayload> handler)
        {
            _handlers.Remove(handler);
        }

        /// <inheritdoc />
        public EventRoutingStrategy EventRoutingStrategy => EventRoutingStrategy.Direct;
    }
}