using System;
using System.Collections.Generic;

namespace Coddee
{
    public class GroupEvent<TPayload> : IEvent
    {
        public GroupEvent()
        {
            _handlers = new Dictionary<string, List<Action<TPayload>>>();
        }

        protected readonly Dictionary<string, List<Action<TPayload>>> _handlers;

        public virtual void Raise(string group, TPayload payload)
        {
            if (_handlers.ContainsKey(group))
                foreach (var handler in _handlers[group])
                {
                    handler?.Invoke(payload);
                }
        }

        public virtual void Subscribe(string group, Action<TPayload> handler)
        {
            var list = _handlers.ContainsKey(group) ? _handlers[group] : (_handlers[group] = new List<Action<TPayload>>());
            list.Add(handler);
        }

        public virtual void Unsubscribe(string group, Action<TPayload> handler)
        {
            if (_handlers.ContainsKey(group))
                _handlers[group].Remove(handler);
        }

        public EventRoutingStrategy EventRoutingStrategy => EventRoutingStrategy.Direct;
    }
}
