using System;
using Microsoft.Extensions.DependencyInjection;

namespace Coddee.AspNet
{
    /// <summary>
    /// An <see cref="IDynamicApiAction"/> implementation for controllers
    /// </summary>
    public class DynamicApiControllersAction : DynamicApiActionBase
    {
        private readonly IContainer _container;

        /// <inheritdoc />
        public DynamicApiControllersAction(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// They type of the controller.
        /// </summary>
        public Type ControllerType { get; set; }

        /// <param name="context"></param>
        /// <inheritdoc />
        protected override object GetInstnaceOwner(object context)
        {
            return ActivatorUtilities.CreateInstance(((AspCoreContainer)_container).ServiceProvider, ControllerType);
        }
    }
}