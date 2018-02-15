// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;


namespace Coddee.Services.ApplicationConsole
{
    /// <summary>
    /// A module that provide the <see cref="IApplicationConsole"/> services.
    /// </summary>
    [Module(BuiltInModules.ApplicationConsole)]
    public class ApplicationConsoleModule : IModule
    {
        /// <inheritdoc/>
        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<IConsoleCommandParser, ConsoleCommandParser>();
            container.RegisterInstance<IApplicationConsole, ApplicationConsoleService>();
            return Task.FromResult(true);
        }
    }
}