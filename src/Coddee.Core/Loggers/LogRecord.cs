// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Loggers
{
    /// <summary>
    /// Log record cause
    /// </summary>
    public enum LogRecordTypes
    {
        /// <summary>
        /// A detailed Debug information
        /// </summary>
        Debug,

        /// <summary>
        /// General information about the application.
        /// </summary>
        Information,

        /// <summary>
        /// A warning that may prevent an error.
        /// </summary>
        Warning,

        /// <summary>
        /// Error in some operation.
        /// </summary>
        Error
    }

    /// <summary>
    /// A log record.
    /// </summary>
    public class LogRecord
    {

        /// <summary>
        /// The record date.
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// The type of information.
        /// </summary>
        public LogRecordTypes Type { get; set; }

        /// <summary>
        /// The record source
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// The content of the log record
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// The exception object in case the record type is Error
        /// </summary>
        public Exception Exception { get; set; }
    }
}