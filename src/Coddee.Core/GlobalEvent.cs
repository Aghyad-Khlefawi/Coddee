// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;

namespace Coddee
{
    public enum EventRoutingStrategy
    {
        //
        // Summary:
        //     The routed event uses a tunneling strategy, where the event instance routes downwards
        //     through the tree, from root to source element.
        Tunnel = 0,
        //
        // Summary:
        //     The routed event uses a bubbling strategy, where the event instance routes upwards
        //     through the tree, from event source to root.
        Bubble = 1,
        //
        // Summary:
        //     The routed event does not route through an element tree, but does support other
        //     routed event capabilities such as class handling, System.Windows.EventTrigger
        //     or System.Windows.EventSetter.
        Direct = 2
    }

    public interface IGlobalEvent 
    {

    }

    public class GlobalEvent<TPayload> : IGlobalEvent
    {
        public GlobalEvent()
        {
            _handlers = new List<Action<TPayload>>();
        }

        protected readonly List<Action<TPayload>> _handlers;

        public virtual void Invoke(TPayload payload)
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
    }
}