// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Microsoft.Practices.Unity;

namespace Coddee.AppBuilder
{
    public class BuildAction
    {

        public BuildAction(string name)
        {
            Name = name;
        }
        public BuildAction(string name, Action<IUnityContainer> action)
            :this(name)
        {
            Action = action;
        }

        public BuildAction(string name, Action<IUnityContainer> action, int defaultInvokeOrder)
            :this(name,action)
        {
            DefaultInvokeOrder = defaultInvokeOrder;
        }

        
        public string Name { get; set; }
        public Action<IUnityContainer> Action { get; set; }

        public int? DefaultInvokeOrder { get; set; }
        public int InvokeOrder { get; set; }

        public bool IsInvoked { get; private set; }

        public void Invoke(IUnityContainer container)
        {
            Action?.Invoke(container);
            Invoked?.Invoke(this);
            IsInvoked = true;
        }

        public event Action<BuildAction> Invoked;
    }
}
