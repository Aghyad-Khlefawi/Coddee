// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Modules;

namespace Coddee.ModuleDefinitions
{
   public class CoreModuleDefinitions
    {
       public static Type[] Modules = new[]
       {
           typeof(EventDispatcherServiceModule),
           typeof(GlobalVariablesServiceModule),
           typeof(LocalizationManagerModule),
       };
   }
}
