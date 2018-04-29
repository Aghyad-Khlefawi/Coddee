using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Coddee.Services;

namespace Coddee.Xamarin.Services.Configuration
{
    /// <summary>
    /// Register the <see cref="IConfigurationManager"/> service.
    /// </summary>
    [Module(BuiltInModules.ConfigurationManager)]
    public class ConfigurationManagerModule : IModule
    {
        /// <inheritdoc />

        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<IConfigurationManager,ConfigurationManager>();
            return Task.CompletedTask;
        }
    }
}
