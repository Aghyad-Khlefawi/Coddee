using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coddee
{
    public class GroupEvent<TPayload> : IEvent
    {
        public GroupEvent()
        {
            _handlers = new Dictionary<string, List<Action<TPayload>>>();
            _asyncHandlers = new Dictionary<string, List<Func<TPayload, Task>>>();
        }

        protected readonly Dictionary<string, List<Action<TPayload>>> _handlers;
        protected readonly Dictionary<string, List<Func<TPayload, Task>>> _asyncHandlers;

        public virtual void Raise(string group, TPayload payload)
        {
            if (_handlers.ContainsKey(group))
                foreach (var handler in _handlers[group])
                {
                    handler?.Invoke(payload);
                }
        }

        public virtual async Task RaiseAsync(string group, TPayload payload)
        {
            Raise(group, payload);
            if (_asyncHandlers.ContainsKey(group))
                foreach (var handler in _asyncHandlers[group])
                {
                    await handler?.Invoke(payload);
                }
        }

        public virtual void Subscribe(string group, Action<TPayload> handler)
        {
            var list = _handlers.ContainsKey(group) ? _handlers[group] : (_handlers[group] = new List<Action<TPayload>>());
            list.Add(handler);
        }
        public virtual void SubscribeAsync(string group, Func<TPayload, Task> handler)
        {
            var list = _asyncHandlers.ContainsKey(group) ? _asyncHandlers[group] : (_asyncHandlers[group] = new List<Func<TPayload, Task>>());
            list.Add(handler);
        }

        public virtual void Unsubscribe(string group, Action<TPayload> handler)
        {
            if (_handlers.ContainsKey(group))
                _handlers[group].Remove(handler);
        }
        public virtual void UnsubscribeAsync(string group, Func<TPayload, Task> handler)
        {
            if (_asyncHandlers.ContainsKey(group))
                _asyncHandlers[group].Remove(handler);
        }
        public EventRoutingStrategy EventRoutingStrategy => EventRoutingStrategy.Direct;
    }
}
