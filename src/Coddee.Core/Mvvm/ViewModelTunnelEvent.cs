namespace Coddee.Mvvm
{
    /// <summary>
    /// Base class for ViewModel event that invokes the handlers in all the child ViewModels.
    /// </summary>
    public class ViewModelTunnelEvent<TPayload> : ViewModelEvent<TPayload>
    {
        /// <inheritdoc />
        public ViewModelTunnelEvent() : base(EventRoutingStrategy.Tunnel)
        {

        }

        /// <inheritdoc />
        public override void Raise(IViewModel sender, TPayload args)
        {
            InvokeChildren(sender, sender, args);
        }

        private void InvokeChildren(IViewModel sender, IViewModel target, TPayload args)
        {
            var childViewModels = _viewModelsManager.GetChildViewModels(target);
            foreach (var childViewModel in childViewModels)
            {
                var handler = GetHandler(childViewModel.ViewModel);
                handler?.Invoke(sender, args);
                InvokeChildren(sender, childViewModel.ViewModel, args);
            }
        }
    }
}