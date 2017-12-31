// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using Coddee.WPF.Services.Dialogs;

namespace Coddee.Services.Dialogs
{
    [Module(BuiltInModules.DialogService)]
    public class DialogServicsModule : IModule
    {
        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<IDialogService, DialogService>();
            container.RegisterInstance<IDialogExplorer, DialogsExplorerViewModel>();
            return Task.FromResult(true);
        }
    }
}