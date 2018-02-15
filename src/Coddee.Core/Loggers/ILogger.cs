// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Loggers
{
    /// <summary>
    /// ILogger interface define the basic functionality of a event logger
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// The minimum level of the record that will be committed.
        /// </summary>
        LogRecordTypes MinimumLevel { get; }

        /// <summary>
        /// Initialize the logger
        /// </summary>
        /// <param name="type">The minimum event level to show</param>
        void Initialize(LogRecordTypes type);

        /// <summary>
        /// Log an event.
        /// </summary>
        void Log(LogRecord record);

        /// <summary>
        /// Log an event.
        /// </summary>
        void Log(string source,string content);

        /// <summary>
        /// Log an event.
        /// </summary>
        void Log(string source,string content,DateTime date);

        /// <summary>
        /// Log an event.
        /// </summary>
        void Log(string source,string content,LogRecordTypes type);

        /// <summary>
        /// Log an event.
        /// </summary>
        void Log(string source,string content,LogRecordTypes type,DateTime date);

        /// <summary>
        /// Log an event.
        /// </summary>
        void Log(string source,Exception exception);

        /// <summary>
        /// Log an event.
        /// </summary>
        void Log(string source,Exception exception,DateTime date);

        /// <summary>
        /// Triggered when a log is received.
        /// </summary>
        event EventHandler<LogRecord> LogRecieved;

        /// <summary>
        /// Sets the <see cref="MinimumLevel"/> property.
        /// </summary>
        void SetLogLevel(LogRecordTypes logLevel);
    }
}
