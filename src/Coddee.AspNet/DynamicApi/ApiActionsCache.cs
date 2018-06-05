using System.Collections.Generic;

namespace Coddee.AspNet
{
    /// <summary>
    /// Keeps instances of used ApiActions.
    /// </summary>
    public class ApiActionsCache
    {
        /// <inheritdoc />
        public ApiActionsCache()
        {
            _actions = new Dictionary<string, IDynamicApiAction>();
        }

        private readonly Dictionary<string, IDynamicApiAction> _actions;

        /// <summary>
        /// Returns an instance of the <see cref="IDynamicApiAction"/> if found and <see langword="null"/> if not.
        /// </summary>
        /// <param name="request">The request object.</param>
        public IDynamicApiAction GetAction(DynamicApiRequest request)
        {
            var key = GetActionKey(request.RequestedActionPath);
            if (_actions.TryGetValue(key, out var action))
            {
                return action;
            }
            return null;
        }

        /// <summary>
        /// Adds an action to the cache.
        /// </summary>
        public void AddAction(IDynamicApiAction action)
        {
            var key = GetActionKey(action.Path);
            if (!_actions.ContainsKey(key))
                _actions.Add(key, action);
        }

        private static string GetActionKey(DynamicApiActionPath actionPath)
        {
            return $"{actionPath.RequestedController}:{actionPath.RequestedAction}";
        }
    }
}