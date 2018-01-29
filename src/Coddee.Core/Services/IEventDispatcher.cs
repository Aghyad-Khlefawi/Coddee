namespace Coddee.Services
{
    public interface IEventDispatcher
    {
        /// <summary>
        /// Gets the event that needs to be invoked or subscribed to.
        /// </summary>
        /// <typeparam name="TResult">The event type.</typeparam>
        /// <returns>The global event object.</returns>
        TResult GetEvent<TResult>() where TResult : class, IEvent, new();
    }
}
