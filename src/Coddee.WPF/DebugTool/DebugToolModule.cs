// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using Coddee.Modules;
using Coddee.Services;


namespace Coddee.WPF.DebugTool
{
    /// <summary>
    /// Registers <see cref="IDebugTool"/> service.
    /// </summary>
    [Module(BuiltInModules.DebugTool,ModuleInitializationTypes.Auto,nameof(ViewModelManagerModule))]
    public class DebugToolModule:IModule
    {
        /// <inheritdoc />
        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<IDebugTool,DebugToolViewModel>();
            return Task.FromResult(true);
        }
    }
}