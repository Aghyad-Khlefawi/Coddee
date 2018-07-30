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
            new XamarinFormsApplication(Guid.Empty, "HR", new CoddeeUnityContainer()).Run(app =>
            {
                app.UseBasicMapper()
                   .UseSingletonRepositoryManager()
                   .UseRESTRepositories(container => new RESTInitializerConfig("http://192.168.1.170:5000/dapi/", RestRepositories.All))
                   .UseNavigationMainPage<MainPageViewModel>();
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
