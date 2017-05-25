// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;

namespace Coddee
{
    public interface IGlobalEvent
    {

    }

    public class GlobalEvent<TPayload> : IGlobalEvent
    {
        public GlobalEvent()
        {
            _handlers = new List<Action<TPayload>>();
        }

        private readonly List<Action<TPayload>> _handlers;

        public void Invoke(TPayload payload)
        {
            foreach (var handler in _handlers)
            {
                handler?.Invoke(payload);
            }
        }

        public void Subscribe(Action<TPayload> handler)
        {
            _handlers.Add(handler);
        }

        public void Unsubscribe(Action<TPayload> handler)
        {
            _handlers.Remove(handler);
        }
    }
}