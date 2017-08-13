// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Coddee.Loggers;
using Coddee.Services;


namespace Coddee.Services
{
    /// <summary>
    /// Responsible for discovering and initializing the application modules
    /// </summary>
    public class ApplicationModulesManager : IApplicationModulesManager
    {
        private const string EventsSource = "ApplicationModulesManager";

        private readonly IContainer _container;
        private readonly ILogger _logger;

        public ApplicationModulesManager(IContainer container, ILogger logger)
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

        public IEnumerable<Module> RegisterModule(params Type[] modules)
        {
            var res = new List<Module>();
            foreach (var type in modules)
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
                res.Add(appModule);
                RegisterModule(appModule);
            }
            return res;
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
        public async Task InitializeModules(params Module[] modules)
        {
            foreach (var module in modules)
            {
                await InitializeModuleWithDependincies(module, null);
            }
        }


        /// <summary>
        /// Calls the initialization method on the modules with auto initialize type
        /// </summary>
        public async Task InitializeAutoModules()
        {
            _logger.Log(EventsSource, $"Initializing auto modules", LogRecordTypes.Debug);
            foreach (var module in _modules.Values.Where(e => e.InitializationType == ModuleInitializationTypes.Auto))
            {
                try
                {
                    await InitializeModuleWithDependincies(module, null);
                }
                catch (Exception ex)
                {
                    _logger?.Log(EventsSource, ex);
                }
            }
        }

        /// <summary>
        /// Aggregate through the modules to resolve and initialize there dependencies then initialize the requested module
        /// </summary>
        /// <param name="module"></param>
        /// <param name="dependencyStack"></param>
        public async Task InitializeModuleWithDependincies(Module module, List<string> dependencyStack)
        {
            if (module.Initialized)
            {
                return;
            }
            if (module.Dependencies == null || !module.Dependencies.Any())
            {
                await InitializeModule(module);
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
                        await InitializeModuleWithDependincies(dep, stack);
                    }
                }
                await InitializeModule(module);
            }
        }

        /// <summary>
        /// Initialize a module by it's name
        /// </summary>
        /// <exception cref="ModuleException"></exception>
        public Task InitializeModule(string moduleName)
        {
            if (!_modules.ContainsKey(moduleName))
                throw new ModuleException($"Module initialization failed because the module {moduleName} not found.");
            return InitializeModuleWithDependincies(_modules[moduleName], null);
        }

        /// <summary>
        /// Initialize a module
        /// </summary>
        /// <exception cref="ModuleException"></exception>
        private async Task InitializeModule(Module module)
        {
            if (module.Instance == null)
                module.Instance = (IModule)Activator.CreateInstance(module.Type);
            _logger.Log(EventsSource, $"Initializing module [{module.Name}]", LogRecordTypes.Debug);
            await module.Instance.Initialize(_container);
            module.Initialized = true;
        }
    }
}