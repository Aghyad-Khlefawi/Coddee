// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;

namespace Coddee.Services.ApplicationConsole
{
   /// <summary>
   /// The console command handler arguments
   /// </summary>
    public class ConsoleCommandArgs : EventArgs
    {
        /// <summary>
        /// The arguments supplied with command
        /// </summary>
        public Dictionary<string,string> Arguments { get; set; }

        /// <summary>
        /// The command object
        /// </summary>
        public ConsoleCommand Command { get; set; }

        /// <summary>
        /// Is the command handled
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// The result returned from the command
        /// </summary>
        public List<string> Result { get; set; }
    }
}