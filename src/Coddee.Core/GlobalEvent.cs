// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;

namespace Coddee
{
    public class GlobalEvent<TPayload> : IEvent
    {
        public GlobalEvent()
        {
            _handlers = new List<Action<TPayload>>();
        }

        protected readonly List<Action<TPayload>> _handlers;

        public virtual void Raise(TPayload payload)
        {
            foreach (var handler in _handlers)
            {
                handler?.Invoke(payload);
            }
        }

        public virtual void Subscribe(Action<TPayload> handler)
        {
            _handlers.Add(handler);
        }

        public virtual void Unsubscribe(Action<TPayload> handler)
        {
            _handlers.Remove(handler);
        }

        public EventRoutingStrategy EventRoutingStrategy => EventRoutingStrategy.Direct;
    }
}