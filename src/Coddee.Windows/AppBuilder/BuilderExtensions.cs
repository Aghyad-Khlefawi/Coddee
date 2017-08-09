// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using Coddee.Loggers;
using Coddee.Services;
using Coddee.Windows.AppBuilder;
using Coddee.Windows.Mapper;


namespace Coddee.AppBuilder
{
    public static class BuilderExtensions
    {

        public static T UseLocalization<T>(
            this T builder,
            string resourceManagerFullPath,
            string resourceManagerAssembly,
            string[] supportedCultures,
            string defaultCluture = "en-US") where T : IApplicationBuilder
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
        public static T UseILMapper<T>(this T builder) where T : IApplicationBuilder
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
        /// <param name="loggerType">Specify which logger to use. Uses Enum flags to specify multiple values</param>
        /// <param name="level">The minimum log level</param>
        /// <returns></returns>
        public static T UseLogger<T>(this T builder,
                                                    LoggerTypes loggerType,
                                                    LogRecordTypes level) where T : IApplicationBuilder
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.LoggerBuildAction((container) =>
            {
                var logger = (LogAggregator)container.Resolve<ILogger>();
                logger.SetLogLevel(level);
                logger.AllowedTypes = loggerType;

                if (loggerType.HasFlag(LoggerTypes.DebugOutput))
                {
                    var debugLogger = container.Resolve<DebugOuputLogger>();
                    debugLogger.Initialize(level);
                    logger.AddLogger(debugLogger, LoggerTypes.DebugOutput);
                }
                if (loggerType.HasFlag(LoggerTypes.File))
                {
                    var fileLogger = container.Resolve<FileLogger>();
                    fileLogger.Initialize(level, "log.txt");
                    logger.AddLogger(fileLogger, LoggerTypes.File);
                }
                if (loggerType.HasFlag(LoggerTypes.ApplicationConsole))
                {
                    var consoleLogger = container.Resolve<ConsoleLogger>();
                    consoleLogger.Initialize(level);
                    logger.AddLogger(consoleLogger, LoggerTypes.ApplicationConsole);
                }
            }));
            return builder;
        }

        public static IConsoleApplicationBuilder UseMain(this IConsoleApplicationBuilder builder,
                                                    Action entryPoint)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.ConsoleMainBuildAction((container) =>
            {
                entryPoint();
            }));
            return builder;
        }

        public static IConsoleApplicationBuilder UseMain(this IConsoleApplicationBuilder builder,
                                                         IEntryPointClass entryPoint)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.ConsoleMainBuildAction((container) =>
            {
                entryPoint.Start();
            }));
            return builder;
        }
        public static IConsoleApplicationBuilder UseMain<T>(this IConsoleApplicationBuilder builder) where T:IEntryPointClass
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.ConsoleMainBuildAction((container) =>
            {
                container.Resolve<T>().Start();
            }));
            return builder;
        }
    }
}
