// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Coddee.Services.GlobalEvents
{
    [Module(BuiltInModules.GlobalEventsService)]
    public class GlobalEventsServiceModule : IModule
    {
        public Task Initialize(IUnityContainer container)
        {
            container.RegisterInstance<IGlobalEventsService, GlobalEventsService>();
            return Task.FromResult(true);
        }
    }
}