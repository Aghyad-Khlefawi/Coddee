// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Diagnostics;

namespace Coddee.Loggers
{
    /// <summary>
    /// A logger that writes to the debug output
    /// </summary>
    public class DebugOuputLogger : LoggerBase
    {
        protected override void CommitLog(LogRecord record)
        {
            Debug.WriteLine(BuildEvent(record));
        }
    }
}