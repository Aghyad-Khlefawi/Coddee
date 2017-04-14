// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Coddee.Loggers;
using Microsoft.Practices.Unity;

namespace Coddee.WPF.Modules
{
    /// <summary>
    /// Responsible for discovering and initializing the application modules
    /// </summary>
    public class ApplicationModulesManager : IApplicationModulesManager
    {
        private const string EventsSource = "ApplicationModulesManager";

        private readonly IUnityContainer _container;
        private readonly ILogger _logger;

        public ApplicationModulesManager(IUnityContainer container, ILogger logger)
        {
            _container = container;
            _container.RegisterInstance<IApplicationModulesManager>(this);
            _logger = logger;
            _modules = new Dictionary<string, Module>();
        }

        private readonly Dictionary<string, Module> _modules;

        /// <summary>
        /// Register a module manually
        /// </summary>
        /// <param name="modules">The modules information</param>
        public IEnumerable<Module> RegisterModule(params Module[] modules)
        {
            foreach (var applicationModule in modules)
            {
                _modules[applicationModule.Name] = applicationModule;
                _logger.Log(EventsSource, $"Module registered [{applicationModule.Name}]", LogRecordTypes.Debug);
            }
            return modules;
        }

        /// <summary>
        /// Search for modules in the executable folder.
        /// Finds the modules based on the ApplicationModuleAttribute
        /// </summary>
        /// <param name="assembliesPrefix">A prefix for the assemblies to look for</param>
        public IEnumerable<Module> DescoverModulesFromAssambles(string assembliesPrefix = null)
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return DescoverModulesFromAssambles(Directory.GetFiles(path, $"{assembliesPrefix}*.dll")
                .Select(Assembly.LoadFile)
                .ToArray());
        }

        /// <summary>
        /// Search for modules in the executable folder.
        /// Finds the modules based on the ApplicationModuleAttribute
        /// </summary>
        /// <param name="assemblies">A specific assemblies to search for modules</param>
        public IEnumerable<Module> DescoverModulesFromAssambles(params Assembly[] assemblies)
        {
            var res = new List<Module>();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttributes();
                    if (!attr.Any() || !attr.Any(e => e is ModuleAttribute))
                        continue;

                    var module = attr.OfType<ModuleAttribute>().First();
                    var appModule = new Module
                    {
                        Type = type,
                        Name = module.ModuleName,
                        Dependencies = module.Dependencies,
                        InitializationType = module.InitializationTypes
                    };
                    if (type.GetInterface(typeof(IModule).Name) == null)
                        throw new ModuleException($"The module {appModule.Name} doesn't implements IMoudle");

                    if (type.GetConstructor(Type.EmptyTypes) == null)
                        throw new ModuleException(
                            $"The module {appModule.Name} must have a parameterless constructor");

                    _logger.Log(EventsSource, $"Module discovered [{appModule.Name}]", LogRecordTypes.Debug);
                    res.Add(appModule);
                }
            }
            return res;
        }

        /// <summary>
        /// Calls the initialization method on the modules
        /// </summary>
        /// <param name="modules"></param>
        public void InitializeModules(params Module[] modules)
        {
            foreach (var module in modules)
            {
                InitializeModuleWithDependincies(module, null);
            }
        }


        /// <summary>
        /// Calls the initialization method on the modules with auto initialize type
        /// </summary>
        public void InitializeAutoModules()
        {
            _logger.Log(EventsSource, $"Initializing auto modules", LogRecordTypes.Debug);
            foreach (var module in _modules.Values.Where(e => e.InitializationType == ModuleInitializationTypes.Auto))
            {
                InitializeModuleWithDependincies(module, null);
            }
        }

        /// <summary>
        /// Aggregate through the modules to resolve and initialize there dependencies then initialize the requested module
        /// </summary>
        /// <param name="module"></param>
        /// <param name="dependencyStack"></param>
        public void InitializeModuleWithDependincies(Module module, List<string> dependencyStack)
        {
            if (module.Dependencies == null || !module.Dependencies.Any())
            {
                InitializeModule(module);
            }
            else
            {
                foreach (var dependency in module.Dependencies)
                {
                    if (!_modules.ContainsKey(dependency))
                        throw new ModuleException(
                            $"The module {module.Name} has an unresolved dependency on {dependency}");

                    var dep = _modules[dependency];

                    if (dependencyStack != null && dependencyStack.Contains(dep.Name))
                        throw new ModuleException(
                            $"Module failed to initialize because of a circular dependency between {module.Name} and {dep.Name}");

                    if (!dep.Initialized)
                    {
                        if (dep.InitializationType == ModuleInitializationTypes.Manual && module.InitializationType ==
                            ModuleInitializationTypes.Auto)
                            throw new ModuleException(
                                $"The module {module.Name} has a dependency on a manually initialized module," +
                                $"An auto initialized module cannot depend on a manually initialized module");

                        var stack = dependencyStack != null ? new List<string>(dependencyStack) : new List<string>();
                        stack.Add(module.Name);
                        InitializeModuleWithDependincies(dep, stack);
                    }
                }
                InitializeModule(module);
            }
        }

        /// <summary>
        /// Initialize a module by it's name
        /// </summary>
        /// <exception cref="ModuleException"></exception>
        public void InitializeModule(string moduleName)
        {
            if (!_modules.ContainsKey(moduleName))
                throw new ModuleException($"Module initialization failed because the module {moduleName} not found.");
            InitializeModuleWithDependincies(_modules[moduleName], null);
        }

        /// <summary>
        /// Initialize a module
        /// </summary>
        /// <exception cref="ModuleException"></exception>
        private void InitializeModule(Module module)
        {
            if (module.Instance == null)
                module.Instance = (IModule)Activator.CreateInstance(module.Type);
            _logger.Log(EventsSource, $"Initializing module [{module.Name}]", LogRecordTypes.Debug);
            module.Instance.Initialize(_container);
        }
    }
}