// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Linq;

namespace Coddee.Loggers
{
    /// <summary>
    /// Redirects it's log to multiple loggers
    /// </summary>
    public class LogAggregator : LoggerBase
    {
        private readonly IList<ILogger> _loggers;
        private readonly IList<LogRecord> _records;

        /// <summary>
        /// The allowed logger types.
        /// </summary>
        public LoggerTypes AllowedTypes { get; set; }

        /// <inheritdoc />
        public LogAggregator()
        {
            _loggers = new List<ILogger>();
            _records = new List<LogRecord>();
        }

        /// <inheritdoc />
        protected override void CommitLog(LogRecord record)
        {
            foreach (var logger in _loggers.ToList())
                logger.Log(record);
        }

        /// <summary>
        /// Adds a logger to the loggers collection
        /// </summary>
        public void AddLogger(ILogger logger, LoggerTypes type)
        {
            if (!AllowedTypes.HasFlag(type))
                return;
            foreach (var log in _records.ToList())
                if (log.Type >= MinimumLevel)
                    logger.Log(log);
            _loggers.Add(logger);
        }
    }
}