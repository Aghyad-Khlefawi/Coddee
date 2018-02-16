using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coddee.Unity;
using Coddee.Xamarin.AppBuilder;
using Unity;
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
                    builder.UseDefaultShell();
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
