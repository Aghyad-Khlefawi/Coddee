// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Coddee.WPF.Modules.Navigation
{
    [Module(BuiltInModules.NavigationService)]
    public class NavigationModule:IModule
    {
        public Task Initialize(IUnityContainer container)
        {
            container.RegisterInstance<INavigationService, NavigationService>();
            return Task.FromResult(true);
        }
    }
}