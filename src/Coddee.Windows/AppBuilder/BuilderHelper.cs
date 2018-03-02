// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.IO;
using Coddee.Loggers;

namespace Coddee.Windows.AppBuilder
{
    /// <summary>
    /// Configurations for using <see cref="ILogger"/> service.
    /// </summary>
    public class LoggerOptions
    {
        /// <param name="loggerType">The logging service type</param>
        /// <param name="level" >The minimum logging level</param>
        public LoggerOptions(LoggerTypes loggerType, LogRecordTypes level)
        {
            LoggerType = loggerType;
            Level = level;
        }

        /// <param name="loggerType">The logging service type</param>
        /// <param name="level" >The minimum logging level</param>
        /// <param name="logFilePath">The full path of the log file</param>
        public LoggerOptions(LoggerTypes loggerType, LogRecordTypes level, string logFilePath) : this(loggerType, level)
        {
            if (string.IsNullOrEmpty(logFilePath))
                logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
            LogFilePath = logFilePath;
        }

        /// <summary>
        /// The logging service type
        /// </summary>

        public LoggerTypes LoggerType { get; set; }
        /// <summary>
        /// The minimum logging level
        /// </summary>
        public LogRecordTypes Level { get; set; }

        /// <summary>
        /// The full path of the log file
        /// <remarks>Required if using <see cref="LoggerTypes.File"/></remarks>
        /// </summary>
        public string LogFilePath { get; set; }

        /// <summary>
        /// if set to true the log files will be compressed and archived after each run.
        /// </summary>
        public bool UseFileCompression { get; set; }
    }

    /// <summary>
    /// Helper class for registering loggers.
    /// </summary>
    public class BuilderHelper
    {
        /// <summary>
        /// Register a new logger.
        /// </summary>
        /// <param name="options">The logger configurations</param>
        /// <param name="container">The application dependency container</param>
        public static void RegisterLoggers(LoggerOptions options, IContainer container)
        {
            var logger = (LogAggregator)container.Resolve<ILogger>();
            var level = options.Level;
            var loggerType = options.LoggerType;
            var filePath =options.LogFilePath;
            logger.SetLogLevel(options.Level);
            logger.AllowedTypes = options.LoggerType;

            if (loggerType.HasFlag(LoggerTypes.DebugOutput))
            {
                var debugLogger = container.Resolve<DebugOuputLogger>();
                debugLogger.Initialize(level);
                logger.AddLogger(debugLogger, LoggerTypes.DebugOutput);
            }
            if (loggerType.HasFlag(LoggerTypes.File))
            {
                var fileLogger = container.Resolve<FileLogger>();

                if (string.IsNullOrEmpty(options.LogFilePath))
                    throw new ArgumentNullException(nameof(options.LogFilePath));

                fileLogger.Initialize(level, filePath, options.UseFileCompression);
                logger.AddLogger(fileLogger, LoggerTypes.File);
            }
            if (loggerType.HasFlag(LoggerTypes.ApplicationConsole))
            {
                var consoleLogger = container.Resolve<ConsoleLogger>();
                consoleLogger.Initialize(level);
                logger.AddLogger(consoleLogger, LoggerTypes.ApplicationConsole);
            }
        }
    }

}


