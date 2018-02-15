// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Text;

namespace Coddee.Loggers
{
    /// <summary>
    /// Base class for logging services.
    /// </summary>
    public abstract class LoggerBase : ILogger
    {
        /// <inheritdoc />
        public LogRecordTypes MinimumLevel { get; protected set; }

        /// <inheritdoc />
        public virtual void Initialize(LogRecordTypes type)
        {
            MinimumLevel = type;
        }

        /// <summary>
        /// Write the log event
        /// </summary>
        /// <param name="record"></param>
        protected abstract void CommitLog(LogRecord record);

        /// <inheritdoc />
        public virtual void Log(LogRecord record)
        {
            if (record.Type >= MinimumLevel)
            { 
                CommitLog(record);
                LogRecieved?.Invoke(this,record);
            }
        }

        /// <inheritdoc />
        public void Log(string source, string content)
        {
            Log(source, content, LogRecordTypes.Information);
        }

        /// <inheritdoc />
        public void Log(string source, string content, DateTime date)
        {
            Log(source, content, LogRecordTypes.Information, date);
        }

        /// <inheritdoc />
        public void Log(string source, string content, LogRecordTypes type)
        {
            Log(source, content, type, DateTime.Now);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void Log(string source, Exception exception)
        {
            Log(source, exception, DateTime.Now);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public event EventHandler<LogRecord> LogRecieved;

        /// <inheritdoc />
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