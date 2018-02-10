using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.Xamarin.Common
{
    public class ViewModelsGroupEvent<TPayload> : IEvent
    {
        public ViewModelsGroupEvent()
        {
            _handlers = new Dictionary<string, List<ViewModelEventHandler<TPayload>>>();
            _asyncHandlers = new Dictionary<string, List<AsyncViewModelEventHandler<TPayload>>>();
        }

        protected readonly Dictionary<string, List<ViewModelEventHandler<TPayload>>> _handlers;
        protected readonly Dictionary<string, List<AsyncViewModelEventHandler<TPayload>>> _asyncHandlers;

        public virtual void Raise(string group, IViewModel sender, TPayload payload)
        {
            lock (_handlers)
            {
                if (_handlers.ContainsKey(group))
                    foreach (var handler in _handlers[group])
                    {
                        handler?.Invoke(sender, payload);
                    }
            }
        }

        public virtual async Task RaiseAsync(string group, IViewModel sender, TPayload payload)
        {
            Raise(group, sender, payload);
            if (_asyncHandlers.ContainsKey(group))
                foreach (var handler in _asyncHandlers[group])
                {
                    await handler?.Invoke(sender, payload);
                }
        }

        public virtual void Subscribe(string group, ViewModelEventHandler<TPayload> handler)
        {
            lock (_handlers)
            {
                var list = _handlers.ContainsKey(group)
                    ? _handlers[group]
                    : (_handlers[group] = new List<ViewModelEventHandler<TPayload>>());
                list.Add(handler);
            }
        }

        public virtual void Unsubscribe(string group, ViewModelEventHandler<TPayload> handler)
        {
            lock (_handlers)
            {
                if (_handlers.ContainsKey(group))
                    _handlers[group].Remove(handler);
            }
        }

        public virtual void SubscribeAsync(string group, AsyncViewModelEventHandler<TPayload> handler)
        {
            lock (_asyncHandlers)
            {
                var list = _asyncHandlers.ContainsKey(group)
                    ? _asyncHandlers[group]
                    : (_asyncHandlers[group] = new List<AsyncViewModelEventHandler<TPayload>>());
                list.Add(handler);
            }
        }

        public virtual void UnsubscribeAsync(string group, AsyncViewModelEventHandler<TPayload> handler)
        {
            lock (_asyncHandlers)
            {
                if (_asyncHandlers.ContainsKey(group))
                    _asyncHandlers[group].Remove(handler);
            }
        }

        public EventRoutingStrategy EventRoutingStrategy => EventRoutingStrategy.Direct;
    }
}