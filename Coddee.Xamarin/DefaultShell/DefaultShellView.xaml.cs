using Coddee.Xamarin.AppBuilder;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Coddee.Xamarin.DefaultShell
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DefaultShellView : ContentPage, IShell
    {
        public DefaultShellView()
        {
            InitializeComponent();
        }
    }
}