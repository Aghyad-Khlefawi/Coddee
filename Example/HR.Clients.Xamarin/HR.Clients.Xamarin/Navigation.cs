using Coddee.Xamarin.Services.Navigation;
using HR.Clients.Xamarin.Main;
using HR.Clients.Xamarin.Settings;

namespace HR.Clients.Xamarin
{
    public static class HRNavigation
    {
        public static INavigationItem[] Navigations =
        {
            new NavigationItem<MainViewModel>("Home",""),
            new NavigationItem<SettingsViewModel>("Settings",""),
        };
    }
}
