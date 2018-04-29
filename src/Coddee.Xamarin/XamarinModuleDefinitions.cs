using System;
using Coddee.Modules;
using Coddee.Xamarin.Services.Configuration;
using Coddee.Xamarin.Services.Navigation;

namespace Coddee.Xamarin
{
   public  class XamarinModuleDefinitions
   {
       public static Type[] Modules =
       {
           typeof(ViewModelManagerModule),
           typeof(NavigationModule),
           typeof(ConfigurationManagerModule),
       };
   }
}
