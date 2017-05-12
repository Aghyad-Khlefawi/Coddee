// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Windows.Input;
using Coddee.Loggers;
using Coddee.WPF.Modules.Console;

namespace Coddee.WPF.Modules
{
    public interface IApplicationConsole:IPresentable
    {
        /// <summary>
        /// Initialize the console
        /// </summary>
        /// <param name="shell"></param>
        /// <param name="logLevellogLevel"></param>
        void Initialize(IShell shell, LogRecordTypes logLevellogLevel);

        /// <summary>
        /// Returns the logger that writes to the console
        /// </summary>
        /// <returns></returns>
        ILogger GetLogger();

        /// <summary>
        /// Toggle the visibility of the console 
        /// </summary>
        void ToggleConsole();

        /// <summary>
        /// Set the condition on which the console visibility will be toggled
        /// </summary>
        /// <param name="toggleCondition"></param>
        void SetToggleCondition(Func<KeyEventArgs, bool> toggleCondition);

        /// <summary>
        /// Add additional commands to the console.
        /// Use must call the <see cref="AddCommandHandler"/> to provide a handler for the command
        /// </summary>
        /// <param name="commands">The console command object</param>
        void AddCommands(params ConsoleCommand[]commands);

        /// <summary>
        /// Provide a handler for a command
        /// </summary>
        /// <param name="command">The console command name</param>
        /// <param name="handler">The command handler</param>
        void AddCommandHandler(string command,EventHandler<ConsoleCommandArgs> handler);

        /// <summary>
        /// Execute a command
        /// </summary>
        void Execute(string commandString);
    }
}