using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            new XamarinApplication("HR Application", new CoddeeUnityContainer()).Run(app =>
                {
                    app.UseDefaultShell();
                });
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