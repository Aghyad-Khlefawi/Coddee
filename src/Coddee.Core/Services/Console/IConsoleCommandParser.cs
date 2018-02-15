// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Services.ApplicationConsole
{
    /// <summary>
    /// An object that can parse console commands.
    /// </summary>
    public interface IConsoleCommandParser
    {
        /// <summary>
        /// Parse a console command string.
        /// </summary>
        /// <returns></returns>
        CommandParseResult ParseCommand(string commandString);


        /// <summary>
        /// Register a console command to the parser.
        /// </summary>
        /// <param name="commands"></param>
        void RegisterCommands(params ConsoleCommand[] commands);
    }
}