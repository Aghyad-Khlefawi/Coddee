// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Loggers;

namespace Coddee.Services.ApplicationConsole
{
    /// <summary>
    /// An <see cref="ILogger"/> implementation that keeps the records in a string.
    /// </summary>
    public class StringLogger : LoggerBase
    {
        /// <inheritdoc />
        protected override void CommitLog(LogRecord record)
        {
            AppendString?.Invoke(BuildEvent(record,true));
        }

        /// <summary>
        /// Called when a commit log is called.
        /// </summary>
        public event Action<string> AppendString;
    }
}