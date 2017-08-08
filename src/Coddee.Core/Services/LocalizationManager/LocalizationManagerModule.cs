// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using Coddee.Services;


namespace Coddee.Modules
{
    [Module(BuiltInModules.LocalizationManager)]
    public class LocalizationManagerModule:IModule
    {
        public Task Initialize(IContainer container)
        {
            var manager = new LocalizationManager();
            manager.Initialize(container);
            LocalizationManager.SetDefaultLocalizationManager(manager);
            container.RegisterInstance<ILocalizationManager>(manager);
            return Task.FromResult(true);
        }
    }
}