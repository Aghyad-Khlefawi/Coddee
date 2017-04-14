// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.Loggers
{
    public enum LogRecordTypes
    {
        Debug,
        Information,
        Warning,
        Error
    }

    public class LogRecord
    {

        public DateTime Date { get; set; }
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