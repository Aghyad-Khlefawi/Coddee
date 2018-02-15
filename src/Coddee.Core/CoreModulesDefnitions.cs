// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Modules;

namespace Coddee.ModuleDefinitions
{
    /// <summary>
    /// The core library module types
    /// </summary>
    public static class CoreModuleDefinitions
    {
        /// <summary>
        /// The modules types.
        /// </summary>
        public static Type[] Modules = new[]
        {
           typeof(EventDispatcherServiceModule),
           typeof(GlobalVariablesServiceModule),
           typeof(LocalizationManagerModule),
       };
    }
}
