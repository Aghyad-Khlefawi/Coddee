using System;

namespace Coddee.AspNet
{
    /// <summary>
    /// An <see cref="IDynamicApiAction"/> implementation for controllers
    /// </summary>
    public class DynamicApiControllersAction : DynamicApiActionBase
    {
        /// <summary>
        /// They type of the controller.
        /// </summary>
        public Type ControllerType { get; set; }

        /// <inheritdoc />
        protected override object GetInstnaceOwner()
        {
            return Activator.CreateInstance(ControllerType);
        }
    }
}