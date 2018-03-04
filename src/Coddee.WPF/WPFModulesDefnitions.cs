// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Notification;
using Coddee.Services.ApplicationConsole;
using Coddee.SQL;
using Coddee.WPF.DebugTool;
using Coddee.Services.Dialogs;
using Coddee.Services.Navigation;
using Coddee.Services.Toast;
using Coddee.Services.ApplicationSearch;

namespace Coddee.ModuleDefinitions
{
   public class WPFModuleDefinitions
    {
       public static Type[] Modules = 
       {
           typeof(ApplicationConsoleModule),
           typeof(DebugToolModule),
           typeof(DialogServicsModule),
           typeof(NavigationModule),
           typeof(ToastServiceModule),
           typeof(SQLDBBrowserModule),
           typeof(ApplicationSearchModule),
           typeof(NotificationModule)
       };
   }
}
