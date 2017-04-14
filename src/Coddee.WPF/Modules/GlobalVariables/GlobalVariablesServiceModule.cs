// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.Services;
using Microsoft.Practices.Unity;

namespace Coddee.WPF.Modules
{
    [Module(BuiltInModules.GlobalVariablesService)]
    public class GlobalVariablesServiceModule:IModule
    {
        public void Initialize(IUnityContainer container)
        {
            container.RegisterInstance<IGlobalVariablesService,GlobalVariablesService>();
        }
    }
}