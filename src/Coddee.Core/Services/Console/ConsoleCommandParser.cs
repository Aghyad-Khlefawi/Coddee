// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;

namespace Coddee.Services.ApplicationConsole
{
    /// <summary>
    /// The command parsing result
    /// </summary>
    public class CommandParseResult
    {
        /// <summary>
        /// The command object
        /// </summary>
        public ConsoleCommand Command { get; set; }
        
        /// <summary>
        /// Is supplied arguments
        /// </summary>
        public Dictionary<string, string> Arguments { get; set; }
    }

    /// <summary>
    /// Console commands parser
    /// </summary>
    public class ConsoleCommandParser : IConsoleCommandParser
    {
        public ConsoleCommandParser()
        {
            _commands = new Dictionary<string, ConsoleCommand>();
        }

        /// <summary>
        /// Supported commands
        /// </summary>
        private readonly Dictionary<string, ConsoleCommand> _commands;

        /// <summary>
        /// Parse a string to a command
        /// </summary>
        /// <param name="commandString">The string to parse.</param>
        /// <returns></returns>
        public CommandParseResult ParseCommand(string commandString)
        {
            var hasArguments = commandString.Contains(" ") && commandString.Contains("/");
            var commandStr = (hasArguments
                    ? commandString.Substring(0, commandString.IndexOf(" ", StringComparison.Ordinal))
                    : commandString).ToLower()
                .Trim();

            if (_commands.ContainsKey(commandStr))
            {
                var res = new CommandParseResult
                {
                    Command = _commands[commandStr]
                };
                if (!hasArguments)
                    return res;
                
                res.Arguments = new Dictionary<string, string>();
                var args = commandString.Replace(commandStr, "").Trim();
                //string parserCommand = (string)args.Clone();
                while (!string.IsNullOrEmpty(args))
                {
                    var arg = GetNextArgument(ref args);
                    if (res.Arguments.ContainsKey(arg.Key))
                        throw new CommandParseException($"The argument {arg.Key} is set multiple times");
                    res.Arguments.Add(arg.Key,arg.Value);
                }
                return res;
            }
            return null;
        }

        /// <summary>
        /// Returns the next argument from the string
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private KeyValuePair<string, string> GetNextArgument(ref string args)
        {
            if (!args.StartsWith("/"))
                throw new CommandParseException("The command argument must be prefixed with '/'. e.g.  'setlanguage /l arabic'");

            bool hasValue = args.Contains(" ");

            var key = hasValue
                ? args.Substring(0, args.IndexOf(" ", StringComparison.Ordinal))
                : args;
            args = args.Replace(key, "").TrimStart();
            if (!hasValue)
            {
                return new KeyValuePair<string, string>(key, null);
            }
            var hasAnotherArgument = args.Contains("/");
            var value = hasAnotherArgument ? args.Substring(0, args.IndexOf(" ", StringComparison.Ordinal)) : args;
            args = args.Replace(value, "").TrimStart();
            return new KeyValuePair<string, string>(key,value);
        }

        /// <summary>
        /// Register supported commands
        /// </summary>
        /// <param name="commands"></param>
        public void RegisterCommands(params ConsoleCommand[] commands)
        {
            foreach (var command in commands)
            {
                if (command.Name.Contains(" ") || command.Name.Contains("/"))
                    throw new ArgumentException("The console name cannot contain spaces or '/'.");
                if (_commands.ContainsKey(command.Name.ToLower()))
                    throw new ArgumentException($"The command {command.Name} is already registered.");

                _commands[command.Name.ToLower()] = command;
            }
        }
    }

    public class CommandParseException : Exception
    {
        public CommandParseException()
        {
        }

        public CommandParseException(string message) : base(message)
        {
        }

        public CommandParseException(string message, Exception inner) : base(message, inner)
        {
        }
        
    }
}