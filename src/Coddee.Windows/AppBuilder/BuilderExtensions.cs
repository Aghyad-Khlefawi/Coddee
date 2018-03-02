// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using Coddee.Services;
using Coddee.Services.Configuration;
using Coddee.Windows.AppBuilder;
using Coddee.Windows.Mapper;


namespace Coddee.AppBuilder
{
    /// <summary>
    /// Builder extension for a windows application.
    /// </summary>
    public static class BuilderExtensions
    {

        /// <summary>
        /// Initialize <see cref="ILocalizationManager"/> service.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="resourceManagerFullPath">Resource files location <remarks>example "HR.Clients.WPF.Properties.Resources"</remarks></param>
        /// <param name="resourceManagerAssembly">The name of the assembly containing the resources <remarks>Example "HR.Clients.WPF.exe"</remarks></param>
        /// <param name="supportedCultures">The available cultures in the resource files <remarks>Example new[] {"ar-SY", "en-US"}</remarks></param>
        /// <param name="defaultCluture">The default culture used in the application <remarks>Example "en-US"</remarks></param>
        /// <returns></returns>
        public static IApplicationBuilder UseLocalization(
            this IApplicationBuilder builder,
            string resourceManagerFullPath,
            string resourceManagerAssembly,
            string[] supportedCultures,
            string defaultCluture = "en-US")
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.LocalizationBuildAction((container) =>
            {
                var localizationManager = container.Resolve<ILocalizationManager>();
                localizationManager.SetCulture(defaultCluture);
                var values = new Dictionary<string, Dictionary<string, string>>();
                var res = new ResourceManager(resourceManagerFullPath,
                                              Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain
                                                                                 .BaseDirectory,
                                                                             resourceManagerAssembly)));
                foreach (var culture in supportedCultures)
                {
                    foreach (DictionaryEntry val in
                        res.GetResourceSet(new CultureInfo(culture), true, true))
                    {
                        if (!values.ContainsKey(val.Key.ToString()))
                            values[val.Key.ToString()] = new Dictionary<string, string>();
                        values[val.Key.ToString()][culture] = val.Value.ToString();
                    }
                }
                localizationManager.AddValues(values);
            }));
            return builder;
        }

        /// <summary>
        /// Use the IL object mapper
        /// </summary>
        public static IApplicationBuilder UseILMapper(this IApplicationBuilder builder)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.MapperBuildAction((container) =>
            {
                container.RegisterInstance<IObjectMapper, ILObjectsMapper>();
            }));
            return builder;
        }






        /// <summary>
        /// Sets the minimum log level to show to the user
        /// </summary>
        /// <returns></returns>
        public static IApplicationBuilder UseLogger(this IApplicationBuilder builder,
                                                    LoggerOptions options)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.LoggerBuildAction((container) =>
            {
                BuilderHelper.RegisterLoggers(options, container);
            }));
            return builder;
        }

        /// <summary>
        /// Set the application entry point
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="entryPoint">Application main method</param>
        /// <returns></returns>
        public static IApplicationBuilder UseMain(this IApplicationBuilder builder,
                                                    Action<IContainer> entryPoint)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.ConsoleMainBuildAction(entryPoint));
            return builder;
        }

        /// <summary>
        /// Set the application entry point
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="entryPoint">The class containing the entry point.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseMain(this IApplicationBuilder builder,
                                                         IEntryPointClass entryPoint)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.ConsoleMainBuildAction(entryPoint.Start));
            return builder;
        }

        /// <summary>
        /// Set the application entry point
        /// </summary>
        /// <param name="builder"></param>
        /// <typeparam name="T">The class containing the entry point</typeparam>
        /// <returns></returns>
        public static IApplicationBuilder UseMain<T>(this IApplicationBuilder builder) where T : IEntryPointClass
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.ConsoleMainBuildAction((container) =>
            {
                container.Resolve<T>().Start(container);
            }));
            return builder;
        }

        /// <summary>
        /// Initialize the configuration manager
        /// </summary>
        public static IApplicationBuilder UseConfigurationFile(
            this IApplicationBuilder builder)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.ConfigFileBuildAction((container) =>
            {
                var config = container.Resolve<IConfigurationManager>();
                config.Initialize(new ConfigurationFile("config", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.cfg")));
            }));
            return builder;
        }

        /// <summary>
        /// Add additional modules to the application.
        /// </summary>
        /// <param name="buillder"></param>
        /// <param name="modulesAssemblies">The names of the assemblies containing the modules.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseModules(this IApplicationBuilder buillder, params string[] modulesAssemblies)
        {
            buillder.BuildActionsCoordinator.AddAction(DefaultBuildActions.DiscoverModulesBuildAction(
                                                                                             container =>
                                                                                             {
                                                                                                 var _modulesManager = container.Resolve<ApplicationModulesManager>();
                                                                                                 if (modulesAssemblies != null)
                                                                                                     foreach (var assembly in modulesAssemblies)
                                                                                                     {
                                                                                                         _modulesManager.RegisterModule(_modulesManager.DescoverModulesFromAssambles(assembly).ToArray());
                                                                                                     }
                                                                                                 _modulesManager.InitializeAutoModules().GetAwaiter().GetResult();
                                                                                             }));
            return buillder;
        }

    }
}
