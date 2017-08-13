// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;

namespace Coddee.Services.ApplicationConsole
{
    /// <summary>
    /// A console command object
    /// </summary>
   public class ConsoleCommand
    {
        /// <summary>
        /// The command name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of what the command does
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The expected command arguments
        /// </summary>
        public Dictionary<string,string> SupportedArguments { get; set; }
    }
}
