using System;
using Coddee.Unity;
using Coddee.Xamarin.AppBuilder;
using Xamarin.Forms;

namespace HR.Clients.Xamarin
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            try
            {
                new XamarinApplication("HR Client", new CoddeeUnityContainer()).Run(builder =>
                {
                    builder.UseNavigation(HRNavigation.Navigations);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
