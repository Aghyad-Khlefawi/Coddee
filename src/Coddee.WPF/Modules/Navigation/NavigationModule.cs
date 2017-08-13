// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using Coddee.Services.ViewModelManager;


namespace Coddee.Services.Navigation
{
    [Module(BuiltInModules.NavigationService, ModuleInitializationTypes.Auto, nameof(ViewModelManagerModule))]
    public class NavigationModule:IModule
    {
        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<INavigationService, NavigationService>();
            return Task.FromResult(true);
        }
    }
}