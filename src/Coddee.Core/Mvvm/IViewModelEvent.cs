namespace Coddee.Mvvm
{
    /// <summary>
    /// An ViewModel object event.
    /// </summary>
    public interface IViewModelEvent<TPayload> : IEvent
    {
        /// <summary>
        /// Raise the event.
        /// </summary>
        /// <param name="sender">The sender ViewModel</param>
        /// <param name="args">The event arguments.</param>
        void Raise(IViewModel sender, TPayload args);


        /// <summary>
        /// Add a handler to the event.
        /// </summary>
        /// <param name="viewModel">The subscriber ViewModel</param>
        /// <param name="handler">The event handler.</param>
        void Subscribe(IViewModel viewModel, ViewModelEventHandler<TPayload> handler);


        /// <summary>
        /// Remove a handler from the event.
        /// </summary>
        /// <param name="viewModel">The subscribed ViewModel.</param>
        void Unsubscribe(IViewModel viewModel);
    }
}