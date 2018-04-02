using System.Threading.Tasks;
using Coddee.Modules;

namespace Coddee.Xamarin.Services.Navigation
{
    /// <summary>
    /// Registers <see cref="INavigationService"/>
    /// </summary>
    [Module(BuiltInModules.NavigationService, ModuleInitializationTypes.Auto, nameof(ViewModelManagerModule))]
    public class NavigationModule : IModule
    {
        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<INavigationService, NavigationService>();
            return Task.FromResult(true);
        }
    }
}
