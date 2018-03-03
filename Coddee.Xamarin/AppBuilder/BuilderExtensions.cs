using System;
using Coddee.AppBuilder;
using Coddee.Mvvm;
using Coddee.Services;
using Coddee.Xamarin.DefaultShell;
using Coddee.Xamarin.Events;
using Xamarin.Forms;

namespace Coddee.Xamarin.AppBuilder
{
    public static class BuilderExtensions
    {
        /// <summary>
        /// Sets a default shell for the Xamarin application
        /// </summary>
        /// <typeparam name="TContent">The main content type to be shown on startup</typeparam>
        /// <returns></returns>
        public static IApplicationBuilder UseDefaultShell(this IApplicationBuilder builder, Action<Page> config = null)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.ShellBuildAction((container) =>
            {
                var xamarinApplication = container.Resolve<XamarinApplication>();
                container.Resolve<IGlobalVariablesService>().GetVariable<UsingDefaultShellGlobalVariable>()
                    .SetValue(true);

                var vmManager = container.Resolve<IViewModelsManager>();
                var shellViewModel = vmManager.CreateViewModel<DefaultShellViewModel>(null);

                container.RegisterInstance<IShellViewModel>(shellViewModel);
                container.RegisterInstance<IDefaultShellViewModel>(shellViewModel);


                var systemApplication = xamarinApplication.GetSystemApplication();

                var shell = (DefaultShellView) shellViewModel.GetView();
                config?.Invoke(shell);
                container.RegisterInstance<IShell>(shell);
                systemApplication.MainPage = shell;

                shellViewModel.UseNavigation =
                    builder.BuildActionsCoordinator.BuildActionExists(BuildActionsKeys.Navigation);
                shellViewModel.Initialize().ContinueWith((t) =>
                {
                    container.Resolve<IEventDispatcher>().GetEvent<ApplicationStartedEvent>()
                        .Raise(xamarinApplication);
                });
            }));
            return builder;
        }
    }
}