// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.Loggers
{
    public abstract class LoggerBase : ILogger
    {
        protected LogRecordTypes _minimumLevel;

        public virtual void Initialize(LogRecordTypes type)
        {
            _minimumLevel = type;
        }

        /// <summary>
        /// Write the log event
        /// </summary>
        /// <param name="record"></param>
        protected abstract void CommitLog(LogRecord record);

        public virtual void Log(LogRecord record)
        {
            if (record.Type >= _minimumLevel)
                CommitLog(record);
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
                Content = BuildExceptionString(exception),
                Source = source,
                Type = LogRecordTypes.Error,
                Date = date,
                Exception = exception
            });
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

        /// <summary>
        /// Build a string that represent the exception object
        /// </summary>
        /// <param name="exception">The exception object</param>
        /// <param name="level">The exception depth for spacing inner exception</param>
        /// <param name="debuginfo">Show debug information (Source and stack trace)</param>
        /// <returns></returns>
        public static string BuildExceptionString(Exception exception, int level = 0, bool debuginfo = false)
        {
            var execptionInfoBuilder = new StringBuilder();
            var append = "";
            if (level != 0)
                for (int i = 0; i < level + 1; i++)
                {
                    append += "\t";
                }
            execptionInfoBuilder.Append($"\n{append}\tException Type : {exception.GetType().Name}");
            execptionInfoBuilder.Append($"\n{append}\tDetails: {exception.Message}");
            if (debuginfo)
            {
                execptionInfoBuilder.Append($"\n{append}\tSource: {exception.Source}");
                execptionInfoBuilder.Append($"\n{append}\tTrace: {exception.StackTrace}");
            }
            execptionInfoBuilder.Append("\n");
            if (exception.InnerException != null)
                execptionInfoBuilder.Append(BuildExceptionString(exception.InnerException, level + 1, debuginfo));
            return execptionInfoBuilder.ToString();
        }
    }
}