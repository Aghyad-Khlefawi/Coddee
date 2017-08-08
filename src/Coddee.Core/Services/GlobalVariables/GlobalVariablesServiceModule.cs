// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using Coddee.Services;


namespace Coddee.Modules
{
    [Module(BuiltInModules.GlobalVariablesService)]
    public class GlobalVariablesServiceModule:IModule
    {
        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<IGlobalVariablesService,GlobalVariablesService>();
            return Task.FromResult(true);
        }
    }
}