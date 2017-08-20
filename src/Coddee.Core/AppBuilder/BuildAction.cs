// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;


namespace Coddee.AppBuilder
{
    /// <summary>
    /// An application build.
    /// </summary>
    public class BuildAction
    {

        public BuildAction(string name)
        {
            Name = name;
        }
        public BuildAction(string name, Action<IContainer> action)
            :this(name)
        {
            Action = action;
        }

        public BuildAction(string name, Action<IContainer> action, int defaultInvokeOrder)
            :this(name,action)
        {
            DefaultInvokeOrder = defaultInvokeOrder;
        }

        /// <summary>
        /// The name of the action
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The action that will be invoked when this build action is called.
        /// </summary>
        public Action<IContainer> Action { get; set; }

        /// <summary>
        /// The default ordered of this action.
        /// </summary>
        public int? DefaultInvokeOrder { get; set; }

        /// <summary>
        /// The order that the action will be called in.
        /// </summary>
        public int InvokeOrder { get; set; }

        /// <summary>
        /// Determine whether this action has been called or not. 
        /// </summary>
        public bool IsInvoked { get; private set; }

        /// <summary>
        /// Execute the build action.
        /// </summary>
        /// <param name="container">The dependency container</param>
        public void Invoke(IContainer container)
        {
            Action?.Invoke(container);
            Invoked?.Invoke(this);
            IsInvoked = true;
        }

        /// <summary>
        /// Triggered when the action is executed.
        /// </summary>
        public event Action<BuildAction> Invoked;
    }
}
