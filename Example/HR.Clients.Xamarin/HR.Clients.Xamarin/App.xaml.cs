using System;
using Coddee.Unity;
using Coddee.Xamarin.AppBuilder;
using Xamarin.Forms;

namespace HR.Clients.Xamarin
{
    public partial class App : Application
    {
        private XamarinApplication _currentApplication;

        public App()
        {
            InitializeComponent();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            if (_currentApplication == null)
                SetupXamarinApplication();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private void SetupXamarinApplication()
        {
            try
            {
                _currentApplication = new XamarinApplication("HR Client", new CoddeeUnityContainer());
                _currentApplication.Run(builder => { builder.UseNavigation(HRNavigation.Navigations); });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}