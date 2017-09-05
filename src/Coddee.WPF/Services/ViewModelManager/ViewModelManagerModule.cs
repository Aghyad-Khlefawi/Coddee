﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;


namespace Coddee.Services.ViewModelManager
{
    [Module(nameof(ViewModelManagerModule))]
    public class ViewModelManagerModule:IModule
    {
        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<IViewModelsManager, ViewModelsManager>();
            return Task.FromResult(true);
        }
    }
}