// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.WPF.Console;
using Microsoft.Practices.Unity;

namespace Coddee.WPF.Modules.Console
{
    [Module(BuiltInModules.ApplicationConsole)]
    public class ApplicationConsoleModule : IModule
    {
        public void Initialize(IUnityContainer container)
        {
            container.RegisterInstance<IApplicationConsole, ApplicationConsole>();
        }
    }
}