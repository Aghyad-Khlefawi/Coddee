using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.Modules;
using Microsoft.Practices.Unity;

namespace Coddee.Modules
{
    [Module(BuiltInModules.LocalizationManager)]
    public class LocalizationManagerModule:IModule
    {
        public Task Initialize(IUnityContainer container)
        {
            var manager = new LocalizationManager();
            manager.Initialize(container);
            LocalizationManager.SetDefaultLocalizationManager(manager);
            container.RegisterInstance<ILocalizationManager>(manager);
            return Task.FromResult(true);
        }
    }
}