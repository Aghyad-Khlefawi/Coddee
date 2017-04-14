// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.WPF.Configuration;
using Coddee.WPF.Modules.Interfaces;
using Microsoft.Practices.Unity;

namespace Coddee.WPF.Modules.Configuration
{
    [Module(BuiltInModules.ConfigurationManager)]
    public class ConfigurationManagerModule:IModule
    {
        public void Initialize(IUnityContainer container)
        {
            container.RegisterInstance<IConfigurationManager, ConfigurationManager>();
        }
    }
}
