// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using Coddee.Services;


namespace Coddee.Modules
{
    /// <summary>
    /// Registers the global variables service
    /// </summary>
    [Module(BuiltInModules.GlobalVariablesService)]
    public class GlobalVariablesServiceModule:IModule
    {
        /// <inheritdoc />
        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<IGlobalVariablesService,GlobalVariablesService>();
            return Task.FromResult(true);
        }
    }
}