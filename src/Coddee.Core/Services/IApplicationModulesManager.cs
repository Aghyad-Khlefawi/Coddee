// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Coddee.Services
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
        /// Search for modules in the executable folder.
        /// Finds the modules based on the ApplicationModuleAttribute
        /// </summary>
        /// <param name="assembliesPrefix">A prefix for the assemblies to look for</param>
        /// <exception cref="ModuleException"></exception>
        IEnumerable<Module> DescoverModulesFromAssambles(string assembliesPrefix = null);

        /// <summary>
        /// Search for modules in the executable folder.
        /// Finds the modules based on the ApplicationModuleAttribute
        /// </summary>
        /// <param name="assemblies">A specific assemblies to search for modules</param>
        /// <exception cref="ModuleException"></exception>
        IEnumerable<Module> DescoverModulesFromAssambles(params Assembly[] assemblies);

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