// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Coddee.Loggers;

namespace Coddee.Services
{
    /// <summary>
    /// Responsible for discovering and initializing the application modules
    /// </summary>
    public class WindowsApplicationModulesManager : ApplicationModulesManager,IWindowsApplicationModulesManager
    {
        private const string EventsSource = "WindowsApplicationModulesManager";

        /// <inheritdoc />
        public WindowsApplicationModulesManager(IContainer container, ILogger logger)
            :base(container,logger)
        {
        }

        /// <inheritdoc />
        public IEnumerable<Module> DescoverModulesFromAssambles(string location, string assembliesPrefix = null)
        {
#if NET46
            string path = Path.GetDirectoryName(location);
            return DescoverModulesFromAssambles(Directory.GetFiles(path, $"{assembliesPrefix}*.dll")
                                                    .Select(e => Assembly.LoadFile(e))
                                          .ToArray());
#elif NETSTANDARD2_0
            throw new NotImplementedException("This is supported in NET framework 4.5 and heigher");
#endif
        }
        /// <inheritdoc />
        public IEnumerable<Module> DescoverModulesFromAssambles(params Assembly[] assemblies)
        {
            var res = new List<Module>();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
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
                    if (type.GetInterfaces().All(e => e == typeof(IModule)))
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

    }
}