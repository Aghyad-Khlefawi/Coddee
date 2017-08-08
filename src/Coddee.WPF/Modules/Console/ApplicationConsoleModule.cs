﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;


namespace Coddee.Services.ApplicationConsole
{
    [Module(BuiltInModules.ApplicationConsole)]
    public class ApplicationConsoleModule : IModule
    {
        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<IApplicationConsole, ApplicationConsoleService>();
            return Task.FromResult(true);
        }
    }
}