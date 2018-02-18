using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coddee.Services;

namespace Coddee.Xamarin.Services.ApplicationModulesManager
{
    /// <summary>
    /// Responsible for discovering and initializing the application modules
    /// </summary>
    public interface IApplicationModulesManager
    {
        /// <summary>
        /// Register a module manually
        /// </summary>
        /// <param name="modules">The module information</param>
        /// <exception cref="ModuleException"></exception>
        IEnumerable<Module> RegisterModule(params Module[] modules);

        /// <summary>
        /// Register a module manually
        /// </summary>
        /// <param name="modules">The module type information</param>
        /// <exception cref="ModuleException"></exception>
        IEnumerable<Module> RegisterModule(params Type[] modules);

        /// <summary>
        /// Calls the initialization method on the modules
        /// </summary>
        /// <param name="modules"></param>
        /// <exception cref="ModuleException"></exception>
        Task InitializeModules(params Module[] modules);

        /// <summary>
        /// Calls the initialization method on the modules with auto initialize type
        /// <exception cref="ModuleException"></exception>
        /// </summary>
        Task InitializeAutoModules();

        /// <summary>
        /// Initialize a module by it's name
        /// </summary>
        /// <exception cref="ModuleException"></exception>
        Task InitializeModule(string moduleName);
    }
}