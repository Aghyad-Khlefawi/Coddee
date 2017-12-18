// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Text;

namespace Coddee.Loggers
{
    public abstract class LoggerBase : ILogger
    {
        public LogRecordTypes MinimumLevel { get; protected set; }

        public virtual void Initialize(LogRecordTypes type)
        {
            MinimumLevel = type;
        }

        /// <summary>
        /// Write the log event
        /// </summary>
        /// <param name="record"></param>
        protected abstract void CommitLog(LogRecord record);

        public virtual void Log(LogRecord record)
        {
            if (record.Type >= MinimumLevel)
            { 
                CommitLog(record);
                LogRecieved?.Invoke(this,record);
            }
        }

        public void Log(string source, string content)
        {
            Log(source, content, LogRecordTypes.Information);
        }

        public void Log(string source, string content, DateTime date)
        {
            Log(source, content, LogRecordTypes.Information, date);
        }

        public void Log(string source, string content, LogRecordTypes type)
        {
            Log(source, content, type, DateTime.Now);
        }

        public void Log(string source, string content, LogRecordTypes type, DateTime date)
        {
            Log(new LogRecord
            {
                Content = content,
                Source = source,
                Type = type,
                Date = date
            });
        }

        public void Log(string source, Exception exception)
        {
            Log(source, exception, DateTime.Now);
        }

        public void Log(string source, Exception exception, DateTime date)
        {
            Log(new LogRecord
            {
                Content = exception.BuildExceptionString(0, MinimumLevel == LogRecordTypes.Debug),
                Source = source,
                Type = LogRecordTypes.Error,
                Date = date,
                Exception = exception
            });
        }

        public event EventHandler<LogRecord> LogRecieved;
        public void SetLogLevel(LogRecordTypes logLevel)
        {

            MinimumLevel = logLevel;
        }

        /// <summary>
        /// Build a string that represent the event
        /// </summary>
        /// <param name="log"></param>
        /// <param name="insertNewLine"></param>
        /// <returns></returns>
        protected string BuildEvent(LogRecord log, bool insertNewLine = false)
        {
            var eventInfoBuilder = new StringBuilder();
            eventInfoBuilder.Append($">[{DateTime.Now}]");
            eventInfoBuilder.Append($"  [{log.Type}]");
            eventInfoBuilder.Append($"  [{log.Source}]");
            eventInfoBuilder.Append($"  {log.Content}");
            if (insertNewLine)
                eventInfoBuilder.Append(Environment.NewLine);
            return eventInfoBuilder.ToString();
        }
    }
}