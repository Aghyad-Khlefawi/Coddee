using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.Modules;
using Coddee.Services.GlobalEvents;

namespace Coddee.ModuleDefinitions
{
   public class CoreModuleDefinitions
    {
       public static Type[] Modules = new[]
       {
           typeof(GlobalEventsServiceModule),
           typeof(GlobalVariablesServiceModule),
           typeof(LocalizationManagerModule),
       };
   }
}
