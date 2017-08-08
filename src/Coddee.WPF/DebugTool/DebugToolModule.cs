﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using Coddee.Services;
using Coddee.Services.ViewModelManager;


namespace Coddee.WPF.DebugTool
{
    [Module(BuiltInModules.DebugTool,ModuleInitializationTypes.Auto,nameof(ViewModelManagerModule))]
    public class DebugToolModule:IModule
    {
        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<IDebugTool,DebugToolViewModel>();
            return Task.FromResult(true);
        }
    }
}