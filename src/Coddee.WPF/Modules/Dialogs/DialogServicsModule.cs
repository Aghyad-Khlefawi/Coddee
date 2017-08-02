// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Coddee.Services.Dialogs
{
    [Module(BuiltInModules.DialogService)]
    public class DialogServicsModule : IModule
    {
        public Task Initialize(IUnityContainer container)
        {
            container.RegisterInstance<IDialogService, DialogService>();
            return Task.FromResult(true);
        }
    }
}