// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Loggers;

namespace Coddee.Services.ApplicationConsole
{
    public class StringLogger : LoggerBase
    {
        protected override void CommitLog(LogRecord record)
        {
            AppendString?.Invoke(BuildEvent(record,true));
        }

        public event Action<string> AppendString;
    }
}