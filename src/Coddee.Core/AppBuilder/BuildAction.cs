// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;


namespace Coddee.AppBuilder
{
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

        
        public string Name { get; set; }
        public Action<IContainer> Action { get; set; }

        public int? DefaultInvokeOrder { get; set; }
        public int InvokeOrder { get; set; }

        public bool IsInvoked { get; private set; }

        public void Invoke(IContainer container)
        {
            Action?.Invoke(container);
            Invoked?.Invoke(this);
            IsInvoked = true;
        }

        public event Action<BuildAction> Invoked;
    }
}
