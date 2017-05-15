// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Threading.Tasks;
using Coddee.WPF;
using Coddee.WPF.Modules;
using Microsoft.Practices.Unity;

namespace Coddee.SQL
{
    [Module(BuiltInModules.SQLDBBrowser, ModuleInitializationTypes.Auto, BuiltInModules.ConfigurationManager)
    ]
    public class SQLDBBrowserModule : IModule
    {
        public Task Initialize(IUnityContainer container)
        {
            container.RegisterInstance<ISQLDBBrowser, SQLDBBrowse>();
            return Task.FromResult(true);
        }
    }
}