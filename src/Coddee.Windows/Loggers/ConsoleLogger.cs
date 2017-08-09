using System;

namespace Coddee.Loggers
{
    public class ConsoleLogger : LoggerBase
    {
        protected override void CommitLog(LogRecord record)
        {
            Console.WriteLine(BuildEvent(record));
        }
    }
}
