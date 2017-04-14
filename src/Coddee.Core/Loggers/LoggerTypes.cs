// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.Loggers
{
    [Flags]
    public enum LoggerTypes
    {
        // No logging
        None = 0,

        // Sends the log to the debug output
        DebugOutput = 1 << 0,

        // Sends the log the application console
        ApplicationConsole = 1 << 1,

        // Writes the log to a file
        File = 1 << 2,
    }
}