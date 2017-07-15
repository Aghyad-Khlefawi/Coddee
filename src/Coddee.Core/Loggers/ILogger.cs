// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.Loggers
{
    /// <summary>
    /// ILogger interface define the basic functionality of a event logger
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Initialize the logger
        /// </summary>
        /// <param name="type">The minimum event level to show</param>
        void Initialize(LogRecordTypes type);

        void Log(LogRecord record);
        void Log(string source,string content);
        void Log(string source,string content,DateTime date);
        void Log(string source,string content,LogRecordTypes type);
        void Log(string source,string content,LogRecordTypes type,DateTime date);
        void Log(string source,Exception exception);
        void Log(string source,Exception exception,DateTime date);

        event EventHandler<LogRecord> LogRecieved;
    }
}
