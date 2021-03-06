﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;

namespace Coddee.Loggers
{
    /// <summary>
    /// An <see cref="ILogger"/> implementation that keeps the log records in memory
    /// </summary>
    public class CacheLogger : LoggerBase
    {
        
        private List<LogRecord> _log;

        /// <inheritdoc />
        public override void Initialize(LogRecordTypes type)
        {
            base.Initialize(type);
            _log = new List<LogRecord>();
        }

        /// <inheritdoc />
        protected override void CommitLog(LogRecord record)
        {
            _log.Add(record);
        }

        /// <summary>
        /// Returns the log records that are in the logger.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LogRecord> GetLog()
        {
            return _log;
        }
    }
}