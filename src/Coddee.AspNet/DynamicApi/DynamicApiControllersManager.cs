using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Coddee.Attributes;

namespace Coddee.AspNet
{
    /// <summary>
    /// Discovers and registers controllers actions.
    /// </summary>
    public class DynamicApiControllersManager
    {
        private readonly IContainer _container;
        private readonly List<IDynamicApiAction> _actions;

        /// <inheritdoc />
        public DynamicApiControllersManager(IContainer container)
        {
            _container = container;
            _actions = new List<IDynamicApiAction>();
        }

        /// <summary>
        /// Returns the registered actions.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IDynamicApiAction> GetRegisteredAction()
        {
            return _actions.AsReadOnly();
        }

        /// <summary>
        /// Register a controller and it's actions.
        /// </summary>
        public void RegisterController(Type type)
        {
            var actions = type.GetMethods().Where(e => Attribute.IsDefined((MemberInfo)e, typeof(ApiActionAttribute)));
            foreach (var action in actions)
            {
                var attribute = action.GetCustomAttribute<ApiActionAttribute>();
                var path = attribute.Path.Split('/');

                var controllerAction = new DynamicApiControllersAction(_container)
                {
                    Path = new DynamicApiActionPath(path[0], path[1]),
                    ControllerType = type,
                    Parameters = DynamicApiActionParameter.GetParameters(action),
                };
                controllerAction.SetMethod(action);
                _actions.Add(controllerAction);
            }
        }
    }
}