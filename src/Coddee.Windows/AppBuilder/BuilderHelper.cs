using Coddee;
using Coddee.Loggers;
using Coddee.Windows.AppBuilder;

namespace Coddee.Windows.AppBuilder
{
    public class LoggerOptions
    {
        public LoggerOptions(LoggerTypes loggerType, LogRecordTypes level)
        {
            LoggerType = loggerType;
            Level = level;
        }

        public LoggerTypes LoggerType { get; set; }
        public LogRecordTypes Level { get; set; }
        public string LogFilePath { get; set; }
    }

}
public class BuilderHelper
{
    public static void RegisterLoggers(LoggerOptions options, IContainer container)
    {
        var logger = (LogAggregator)container.Resolve<ILogger>();
        var level = options.Level;
        var loggerType = options.LoggerType;
        var filePath = string.IsNullOrWhiteSpace(options.LogFilePath) ? "log" : options.LogFilePath;
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
            fileLogger.Initialize(level, filePath);
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

