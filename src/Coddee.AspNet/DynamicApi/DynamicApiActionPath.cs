namespace Coddee.AspNet
{
    /// <summary>
    /// Contains the route information of a <see cref="DynamicApiRequest"/>
    /// </summary>
    public class DynamicApiActionPath
    {
        /// <inheritdoc />
        public DynamicApiActionPath(string requestedController, string requestedAction)
        {
            RequestedController = requestedController.ToLower();
            RequestedAction = requestedAction.ToLower();
        }

        /// <summary>
        /// Controller that requested by the URL.
        /// </summary>
        public string RequestedController { get; }

        /// <summary>
        /// Action that requested by the URL.
        /// </summary>
        public string RequestedAction { get; }

    }
}