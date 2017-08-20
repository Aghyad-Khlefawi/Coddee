﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;

namespace Coddee.Services.ApplicationSearch
{
    [Module(nameof(ApplicationSearchModule))]
   public class ApplicationSearchModule:IModule
    {
        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<IApplicationSearchService,ApplicationSearchService>();
            container.RegisterType<IApplicationQuickSearch,SearchViewModel>();
            return Task.FromResult(true);
        }
    }
}
