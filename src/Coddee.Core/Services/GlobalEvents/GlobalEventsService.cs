// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Concurrent;

namespace Coddee.Services
{
    public class GlobalEventsService : IGlobalEventsService
    {
        public GlobalEventsService()
        {
            _events = new ConcurrentDictionary<Type, IGlobalEvent>();
        }

        public ConcurrentDictionary<Type, IGlobalEvent> _events;

        public TResult GetEvent<TResult>() where TResult : class, IGlobalEvent, new()
        {
            IGlobalEvent globalEvent = null;
            if (!_events.TryGetValue(typeof(TResult), out globalEvent))
            {
                globalEvent = new TResult();
                _events.TryAdd(typeof(TResult), globalEvent);
            }
            return (TResult) globalEvent;
        }
    }
}