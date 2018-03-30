using Microsoft.Practices.Unity;
using Xamarin.Forms;

namespace HR.Clients.Xamarin
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new ContentPage{Title = "Main Page - Welcome"};
            try
            {
                var c = new UnityContainer();
            }
            catch (System.Exception ex)
            {

                throw;
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}