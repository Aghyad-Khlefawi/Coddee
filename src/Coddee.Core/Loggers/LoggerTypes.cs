// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Loggers
{
    /// <summary>
    /// Type of logging provided by the application.
    /// </summary>
    [Flags]
    public enum LoggerTypes
    {
        /// <summary>
        /// No logging
        /// </summary>
        None = 0,

        /// <summary>
        /// Sends the log to the debug output
        /// </summary>
        DebugOutput = 1 << 0,

        /// <summary>
        /// Sends the log the application console
        /// </summary>
        ApplicationConsole = 1 << 1,

        /// <summary>
        /// Writes the log to a file
        /// </summary>
        File = 1 << 2,
    }
}