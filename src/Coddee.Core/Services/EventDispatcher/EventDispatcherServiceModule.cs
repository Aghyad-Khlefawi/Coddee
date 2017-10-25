// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using Coddee.Services;


namespace Coddee.Modules
{
    [Module(BuiltInModules.EventDispatcher)]
    public class EventDispatcherServiceModule : IModule
    {
        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<IEventDispatcher, EventDispatcher>();
            return Task.FromResult(true);
        }
    }
}