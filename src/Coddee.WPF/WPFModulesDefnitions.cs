using System;
using Coddee.Services.Configuration;
using Coddee.SQL;
using Coddee.WPF.DebugTool;
using Coddee.WPF.Modules.Console;
using Coddee.WPF.Modules.Dialogs;
using Coddee.WPF.Modules.Navigation;
using Coddee.WPF.Modules.Toast;

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
       };
   }
}
