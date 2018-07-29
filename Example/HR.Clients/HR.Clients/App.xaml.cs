using System;
using Coddee.AppBuilder;
using Coddee.Unity;
using Coddee.Xamarin.Forms;
using HR.Clients.Components;
using HR.Data.REST;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HR.Clients
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var container = new CoddeeUnityContainer();
            new XamarinFormsApplication(Guid.Empty, "HR", container).Run(app =>
            {
                app.UseBasicMapper()
                   .UseSingletonRepositoryManager()
                   .UseRESTRepositories(config => new RESTInitializerConfig("http://localhost:15297/dapi/", null, RestRepositories.All, null))
                   .UseMainPage<MainPageViewModel>();
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
