using System.Collections.Generic;

namespace Coddee.WPF
{
    public class ViewModelsGroupEvent<TPayload> : IEvent
    {
        public ViewModelsGroupEvent()
        {
            _handlers = new Dictionary<string, List<ViewModelEventHandler<TPayload>>>();
        }

        protected readonly Dictionary<string, List<ViewModelEventHandler<TPayload>>> _handlers;

        public virtual void Raise(string group, IViewModel sender, TPayload payload)
        {
            if (_handlers.ContainsKey(group))
                foreach (var handler in _handlers[group])
                {
                    handler?.Invoke(sender,payload);
                }
        }

        public virtual void Subscribe(string group, ViewModelEventHandler<TPayload> handler)
        {
            var list = _handlers.ContainsKey(group) ? _handlers[group] : (_handlers[group] = new List<ViewModelEventHandler<TPayload>>());
            list.Add(handler);
        }

        public virtual void Unsubscribe(string group, ViewModelEventHandler<TPayload> handler)
        {
            if (_handlers.ContainsKey(group))
                _handlers[group].Remove(handler);
        }

        public EventRoutingStrategy EventRoutingStrategy => EventRoutingStrategy.Direct;
    }
}
