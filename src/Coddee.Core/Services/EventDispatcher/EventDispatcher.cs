// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Concurrent;

namespace Coddee.Services
{
    /// <summary>
    /// A service that manages the <see cref="IEvent"/> objects.
    /// </summary>
    public class EventDispatcher : IEventDispatcher
    {
        /// <inheritdoc />
        public EventDispatcher()
        {
            _events = new ConcurrentDictionary<Type, IEvent>();
        }
        

        /// <summary>
        /// The used events instances.
        /// </summary>
        public ConcurrentDictionary<Type, IEvent> _events;

        /// <inheritdoc />
        public TResult GetEvent<TResult>() where TResult : class, IEvent, new()
        {
            lock (_events)
            {
                if (!_events.TryGetValue(typeof(TResult), out var globalEvent))
                {
                    globalEvent = new TResult();
                    _events.TryAdd(typeof(TResult), globalEvent);
                }
                return (TResult) globalEvent;
            }
        }
    }
}