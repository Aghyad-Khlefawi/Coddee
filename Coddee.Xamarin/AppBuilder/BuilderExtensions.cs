using System;
using System.Collections.Generic;
using Coddee.AppBuilder;
using Coddee.Mvvm;
using Coddee.Services;
using Coddee.Xamarin.DefaultShell;
using Coddee.Xamarin.Events;
using Coddee.Xamarin.Services.Navigation;
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

        /// <summary>
        /// Sets a default Navigation for the Xamarin
        /// </summary>
        /// <returns></returns>
        public static IApplicationBuilder UseNavigation(
            this IApplicationBuilder builder,
            params INavigationItem[] navigationItems)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.NavigationBuildAction((container) =>
            {
                var xamarinApplication = container.Resolve<XamarinApplication>();
                var systemApplication = xamarinApplication.GetSystemApplication();
                var navs = new List<INavigationItem>();
                var nav = container.Resolve<INavigationService>();
                navs.AddRange(navigationItems);
                nav.Initialize(navs);
                systemApplication.MainPage = ((NavigationService) nav).GetDefaultView();
                container.Resolve<IEventDispatcher>().GetEvent<ApplicationStartedEvent>()
                    .Raise(xamarinApplication);
            }));
            return builder;
        }
    }
}