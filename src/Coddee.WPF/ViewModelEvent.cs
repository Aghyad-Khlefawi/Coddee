using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.WPF
{
    public interface IViewModelEvent
    {
        EventRoutingStrategy EventRoutingStrategy { get; }
    }
    public interface IViewModelEvent<TPayload> :IViewModelEvent
    {
        ViewModelEventHandler<TPayload> GetHandler(IViewModel viewModel);
        void Subscribe(IViewModel viewModel, ViewModelEventHandler<TPayload> handler);
        void Unsubscribe(IViewModel viewModel);
    }
    public class ViewModelEvent<TPayload>: IViewModelEvent<TPayload>
    {
        protected ViewModelEvent(EventRoutingStrategy routingStrategy)
        {
            _handlers = new Dictionary<IViewModel, ViewModelEventHandler<TPayload>>();
            EventRoutingStrategy = routingStrategy;
        }

        protected readonly Dictionary<IViewModel,ViewModelEventHandler<TPayload>> _handlers;
        public EventRoutingStrategy EventRoutingStrategy { get; protected set; }


        public ViewModelEventHandler<TPayload> GetHandler(IViewModel viewModel)
        {
            if(_handlers.ContainsKey(viewModel))
            return _handlers[viewModel];
            return null;
        }
        
        public virtual void Subscribe(IViewModel viewModel,ViewModelEventHandler<TPayload> handler)
        {
            _handlers.Add(viewModel,handler);
        }

        public virtual void Unsubscribe(IViewModel viewModel)
        {
            _handlers.Remove(viewModel);
        }
    }

}
