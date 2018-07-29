using System;
using Coddee.AppBuilder;
using Coddee.Unity;
using Coddee.Xamarin.Forms;
using HR.Clients.Components;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace HR.Clients
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

            var container = new CoddeeUnityContainer();
            var app = new XamarinFormsApplication(Guid.Empty, "HR");
            var appBuilder = new ApplicationBuilder(app, container);

            appBuilder.UseBasicMapper();
            var main = container.Resolve<MainPageViewModel>();
            MainPage = main.GetDefaultView();

           // MainPage = new MainPage();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
