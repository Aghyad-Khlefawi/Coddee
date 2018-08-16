// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Coddee.Loggers;

namespace Coddee.Services
{
    /// <summary>
    /// Responsible for discovering and initializing the application modules
    /// </summary>
    public class ApplicationModulesManager : IApplicationModulesManager
    {
        private const string EventsSource = "ApplicationModulesManager";

        protected readonly IContainer _container;
        protected readonly ILogger _logger;

        /// <inheritdoc />
        public ApplicationModulesManager(IContainer container, ILogger logger)
        {
            _container = container;
            _logger = logger;
            _modules = new Dictionary<string, Module>();
        }

        private readonly Dictionary<string, Module> _modules;

        /// <inheritdoc />
        public IEnumerable<Module> RegisterModule(params Module[] modules)
        {
            foreach (var applicationModule in modules)
            {
                _modules[applicationModule.Name] = applicationModule;
                _logger.Log(EventsSource, $"Module registered [{applicationModule.Name}]", LogRecordTypes.Debug);
            }
            return modules;
        }

        /// <inheritdoc />
        public IEnumerable<Module> RegisterModule(params Type[] modules)
        {
            var res = new List<Module>();
            foreach (var type in modules)
            {
                var attr = type.GetTypeInfo().GetCustomAttributes();
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

      /// <inheritdoc />
        public async Task InitializeModules(params Module[] modules)
        {
            foreach (var module in modules)
            {
                await InitializeModuleWithDependincies(module, null);
            }
        }


        /// <inheritdoc />
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