using System;
using Coddee.Services.Configuration;

namespace Coddee.ModuleDefinitions
{
   public class WindowsModuleDefinitions
    {
       public static Type[] Modules = new[]
       {
           typeof(ConfigurationManagerModule),
       };
   }
}
