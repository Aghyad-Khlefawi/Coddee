using System.Threading.Tasks;

namespace Coddee.Xamarin.Services.ViewModelManager
{
    [Module(nameof(ViewModelManagerModule))]
    public class ViewModelManagerModule : IModule
    {
        public Task Initialize(IContainer container)
        {
            container.RegisterInstance<IViewModelsManager, ViewModelsManager>();
            return Task.FromResult(true);
        }
    }
}