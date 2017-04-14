// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.Loggers
{
    public class CacheLogger : LoggerBase
    {
        
        private List<LogRecord> _log;

        public override void Initialize(LogRecordTypes type)
        {
            base.Initialize(type);
            _log = new List<LogRecord>();
        }

        protected override void CommitLog(LogRecord record)
        {
            _log.Add(record);
        }

        public IEnumerable<LogRecord> GetLog()
        {
            return _log;
        }
    }
}