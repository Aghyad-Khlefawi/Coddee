// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using Coddee.Services;
using Microsoft.Practices.Unity;

namespace Coddee.WPF.DebugTool
{
    [Module(BuiltInModules.DebugTool)]
    public class DebugToolModule:IModule
    {
        public Task Initialize(IUnityContainer container)
        {
            container.RegisterInstance<IDebugTool,DebugToolViewModel>();
            return Task.FromResult(true);
        }
    }
}