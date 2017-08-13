// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;


namespace Coddee.Services.Toast
{
    [Module(BuiltInModules.ToastService)]
    public class ToastServiceModule : IModule
    {
        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<IToastService, ToastService>();
            return Task.FromResult(true);
        }
    }
}