// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Microsoft.Practices.Unity;

namespace Coddee.WPF.Modules.Navigation
{
    [Module(BuiltInModules.NavigationService)]
    public class NavigationModule:IModule
    {
        public void Initialize(IUnityContainer container)
        {
            container.RegisterInstance<INavigationService, NavigationService>();
        }
    }
}