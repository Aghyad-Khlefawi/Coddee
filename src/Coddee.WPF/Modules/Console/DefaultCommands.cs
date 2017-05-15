// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;

namespace Coddee.WPF.Modules.Console
{
    /// <summary>
    /// The default commands included in the library
    /// </summary>
    public static class DefaultCommands
    {
        public static ConsoleCommand RestartCommand = new ConsoleCommand
        {
            Name = "restart",
            Description = "Restart the application."
        };

        public static ConsoleCommand HelpCommand = new ConsoleCommand
        {
            Name = "help",
            Description = "Shows the available commands."
        };

        public static ConsoleCommand ExitCommand = new ConsoleCommand
        {
            Name = "exit",
            Description = "Shutdown the application."
        };

        public static ConsoleCommand ShowGlobalsCommand = new ConsoleCommand
        {
            Name = "showglobals",
            Description = "Shows the current values of the global variables."
        };
        public static ConsoleCommand ClearCommand = new ConsoleCommand
        {
            Name = "clear",
            Description = "Clears the console."
        };
        public static ConsoleCommand SetScreenCommand = new ConsoleCommand
        {
            Name = "setscreen",
            Description = "Moves the shell to another monitor when using multiple monitors.",
            SupportedArguments = new Dictionary<string, string>
            {
                {"/i", "The screen index (Start index is 0)."}
            }
        };
        public static ConsoleCommand SetLanguageCommand = new ConsoleCommand
        {
            Name = "setlanguage",
            Description = "Changes the language of the localization manager.",
            SupportedArguments = new Dictionary<string, string>
            {
                {"/l", "The new language."}
            }
        };
        public static ConsoleCommand CMDCommand = new ConsoleCommand
        {
            Name = "cmd",
            Description = "Execute a command on the windows cmd.",
            SupportedArguments = new Dictionary<string, string>
            {
                {"/c", "The command to execute."}
            }
        };
    }
}