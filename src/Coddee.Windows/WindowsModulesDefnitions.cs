// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Services.Configuration;

namespace Coddee.ModuleDefinitions
{
    /// <summary>
    /// The windows library module types
    /// </summary>
    public class WindowsModuleDefinitions
    {
        /// <summary>
        /// The modules types.
        /// </summary>
        public static Type[] Modules = {
           typeof(ConfigurationManagerModule),
       };
    }
}
