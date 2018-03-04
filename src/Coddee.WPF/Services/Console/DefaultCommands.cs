// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;

namespace Coddee.Services.ApplicationConsole
{
    /// <summary>
    /// The default commands included in the library
    /// </summary>
    public static class DefaultCommands
    {
        /// <summary>
        /// Restart the application.
        /// </summary>
        public static ConsoleCommand RestartCommand = new ConsoleCommand
        {
            Name = "restart",
            Description = "Restart the application."
        };


        /// <summary>
        /// Shows the available commands.
        /// </summary>
        public static ConsoleCommand HelpCommand = new ConsoleCommand
        {
            Name = "help",
            Description = "Shows the available commands."
        };

        /// <summary>
        /// Shutdown the application.
        /// </summary>
        public static ConsoleCommand ExitCommand = new ConsoleCommand
        {
            Name = "exit",
            Description = "Shutdown the application."
        };

        /// <summary>
        /// Clears the console.
        /// </summary>
        public static ConsoleCommand ClearCommand = new ConsoleCommand
        {
            Name = "clear",
            Description = "Clears the console."
        };

        /// <summary>
        /// Moves the shell to another monitor when using multiple monitors.
        /// </summary>
        public static ConsoleCommand SetScreenCommand = new ConsoleCommand
        {
            Name = "setscreen",
            Description = "Moves the shell to another monitor when using multiple monitors.",
            SupportedArguments = new Dictionary<string, string>
            {
                {"/i", "The screen index (Start index is 0)."}
            }
        };

        /// <summary>
        /// Changes the language of the localization manager.
        /// </summary>
        public static ConsoleCommand SetLanguageCommand = new ConsoleCommand
        {
            Name = "setlanguage",
            Description = "Changes the language of the localization manager.",
            SupportedArguments = new Dictionary<string, string>
            {
                {"/l", "The new language."}
            }
        };

        /// <summary>
        /// Changes the resolution of the application shell.
        /// </summary>
        public static ConsoleCommand SetResolutionCommand = new ConsoleCommand
        {
            Name = "setres",
            Description = "Changes the resolution of the application shell.",
            SupportedArguments = new Dictionary<string, string>
            {
                {"/r", "The new resolution like 1080x1920 or 'FullScreen'."}
            }
        };

        /// <summary>
        /// Execute a command on the windows cmd.
        /// </summary>
        public static ConsoleCommand CMDCommand = new ConsoleCommand
        {
            Name = "cmd",
            Description = "Execute a command on the windows cmd.",
            SupportedArguments = new Dictionary<string, string>
            {
                {"/c", "The command to execute."}
            }
        };

        /// <summary>
        /// All the default commands in the library.
        /// </summary>
        public static ConsoleCommand[] AllCommands = new[]
        {
            DefaultCommands.RestartCommand,
            DefaultCommands.HelpCommand,
            DefaultCommands.ClearCommand,
            DefaultCommands.CMDCommand,
            DefaultCommands.SetScreenCommand,
            DefaultCommands.SetLanguageCommand,
            DefaultCommands.SetResolutionCommand,
            DefaultCommands.ExitCommand
        };
    }
}