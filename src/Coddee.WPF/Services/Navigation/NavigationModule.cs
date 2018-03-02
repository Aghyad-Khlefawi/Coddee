// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using Coddee.Modules;



namespace Coddee.Services.Navigation
{
    /// <summary>
    /// Registers <see cref="INavigationService"/>
    /// </summary>
    [Module(BuiltInModules.NavigationService, ModuleInitializationTypes.Auto, nameof(ViewModelManagerModule))]
    public class NavigationModule:IModule
    {
        /// <inheritdoc />
        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<INavigationService, NavigationService>();
            return Task.FromResult(true);
        }
    }
}