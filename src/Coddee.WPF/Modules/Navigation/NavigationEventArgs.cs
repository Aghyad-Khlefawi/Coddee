using Coddee.WPF.Navigation;

namespace Coddee.WPF.Modules.Navigation
{
   public class NavigationEventArgs
    {
        public INavigationItem OldLocation { get; set; }
        public INavigationItem NewLocation { get; set; }
    }
}
