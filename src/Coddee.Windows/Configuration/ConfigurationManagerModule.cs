// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;


namespace Coddee.Services.Configuration
{
    /// <summary>
    /// Register the <see cref="IConfigurationManager"/> service.
    /// </summary>
    [Module(BuiltInModules.ConfigurationManager)]
    public class ConfigurationManagerModule:IModule
    {
        /// <inheritdoc />
        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<IConfigurationManager, ConfigurationManager>();
            return Task.FromResult(true);
        }
    }
}
