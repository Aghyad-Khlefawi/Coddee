using Coddee.Xamarin.AppBuilder;
using Coddee.Xamarin.Common;
namespace Coddee.Xamarin.DefaultShell
{
    /// <summary>
    /// The default shell viewModel
    /// this viewModel will be used if you don't specify a custom shell at the application build
    /// </summary>
    public class DefaultShellViewModel : ViewModelBase<DefaultShellView>, IDefaultShellViewModel
    {
        

        private bool _useNavigation;
        public bool UseNavigation
        {
            get { return _useNavigation; }
            set { SetProperty(ref this._useNavigation, value); }
        }

      
    }
}