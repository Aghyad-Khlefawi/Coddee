using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.Loggers;

namespace Coddee.Windows.AppBuilder
{
    public class BuilderHelper
    {
        public static void RegisterLoggers(LoggerTypes loggerType, LogRecordTypes level, IContainer container)
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
        }

    }
}
