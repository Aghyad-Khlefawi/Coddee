// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Coddee.Loggers;
using Coddee.Notification;
using Coddee.Services;
using Coddee.WPF.DefaultShell;
using Coddee.WPF.Events;
using Coddee.Services.Navigation;
using Coddee.WPF;
using Coddee.WPF.Security;


namespace Coddee.AppBuilder
{
    public static class BuilderExtensions
    {
        /// <summary>
        /// Add a console to the application
        /// </summary>
        /// <param name="toggleCondition">A function that is executed on the shell KeyDown event show return true to toggle the console</param>
        /// <returns></returns>
        public static IApplicationBuilder UseApplicationConsole(this IApplicationBuilder builder,
                                                                   Func<KeyEventArgs, bool> toggleCondition)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.AppConsoleBuildAction((container) =>
                  {
                      var shell = container.Resolve<IShell>();
                      if (shell == null)
                          throw new ApplicationBuildException("The method must be called after the UseShell method");

                      var applicationConsole = container.Resolve<IApplicationConsole>();
                      var logger = (LogAggregator)container.Resolve<ILogger>();
                      applicationConsole.Initialize(shell, logger.MinimumLevel);

                      //Add the console logger to loggers collection
                      logger.AddLogger(applicationConsole.GetLogger(), LoggerTypes.ApplicationConsole);

                      //Sets the Shell KeyDown event handler to toggle the console visibility
                      applicationConsole.SetToggleCondition(toggleCondition);
                  }));
            return builder;
        }

        /// <summary>
        /// Add a console to the application
        /// </summary>
        /// <param name="toggleCondition">A function that is executed on the shell KeyDown event show return true to toggle the console</param>
        /// <returns></returns>
        public static IApplicationBuilder UseCoddeeDebugTool(this IApplicationBuilder builder,
                                                                Func<KeyEventArgs, bool> toggleCondition)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.DebugToolBuildAction(async (container) =>
                {
                    var shell = container.Resolve<IShell>();
                    if (shell == null)
                        throw new
                            ApplicationBuildException("The method must be called after the UseShell method");
                    var debugTool = container.Resolve<IDebugTool>();

                    await debugTool.Initialize();

                    debugTool.SetToggleCondition(toggleCondition);
                }));
            return builder;
        }




        /// <summary>
        /// Sets a default shell for the WPF application
        /// </summary>
        /// <typeparam name="TContent">The main content type to be shown on startup</typeparam>
        /// <returns></returns>
        public static IApplicationBuilder UseDefaultShell<TContent>(
            this IApplicationBuilder builder,
            WindowState state = WindowState.Maximized,
            Action<Window> config = null)
            where TContent : IPresentable
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.ShellBuildAction((container) =>
            {
                var wpfApplication = container.Resolve<WPFApplication>();
                var shellViewModel = BuildDefaultShell<TContent>(builder, state, config, container);
                shellViewModel.Initialize().ContinueWith((t) =>
                {
                    container.Resolve<IEventDispatcher>().GetEvent<ApplicationStartedEvent>().Raise(wpfApplication);
                });
                wpfApplication.ShowWindow();
            }));
            return builder;
        }

        /// <summary>
        /// Sets a default shell for the WPF application
        /// </summary>
        /// <typeparam name="TContent">The main content type to be shown on startup</typeparam>
        /// <typeparam name="TLogin"></typeparam>
        /// <returns></returns>
        public static IApplicationBuilder UseDefaultShellWithLogin<TContent, TLogin>(
            this IApplicationBuilder builder,
            WindowState state = WindowState.Maximized,
            Action<Window> config = null)
            where TContent : IPresentable
            where TLogin : ILoginViewModel
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.ShellBuildAction((container) =>
            {
                var wpfApplication = container.Resolve<WPFApplication>();
                var shellViewModel = BuildDefaultShell<TContent>(builder, state, config, container);
                var loginViewModel = container.Resolve<TLogin>();
                BuildLogin(container, wpfApplication, shellViewModel, loginViewModel);
            }));
            return builder;
        }



        /// <summary>
        /// Sets a custom shell for the WPF application
        /// </summary>
        /// <typeparam name="TShellViewModel"></typeparam>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomShell<TShellViewModel>(this IApplicationBuilder builder,
                                                                             Action<Window> config = null)
            where TShellViewModel : IShellViewModel
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.ShellBuildAction((container) =>
            {
                var wpfApplication = container.Resolve<WPFApplication>();
                var shellViewModel = BuildCustomShell<TShellViewModel>(config, container);
                shellViewModel.Initialize().ContinueWith((t) =>
                {
                    container.Resolve<IEventDispatcher>().GetEvent<ApplicationStartedEvent>().Raise(wpfApplication);
                });
                wpfApplication.ShowWindow();
            }));
            return builder;
        }

        /// <summary>
        /// Sets a custom shell for the WPF application
        /// </summary>
        /// <typeparam name="TShellViewModel"></typeparam>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomShellWithLogin<TShellViewModel, TLogin>(this IApplicationBuilder builder,
                                                                                              Action<Window> config = null)
            where TShellViewModel : IShellViewModel
            where TLogin : ILoginViewModel
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.ShellBuildAction((container) =>
            {
                var wpfApplication = container.Resolve<WPFApplication>();
                var shellViewModel = BuildCustomShell<TShellViewModel>(config, container);
                var loginViewModel = container.Resolve<TLogin>();
                BuildLogin(container, wpfApplication, shellViewModel, loginViewModel);
            }));
            return builder;
        }

        private static void BuildLogin<TShellViewModel, TLogin>(IContainer container, WPFApplication wpfApplication, TShellViewModel shellViewModel, TLogin loginViewModel)
            where TShellViewModel : IShellViewModel
            where TLogin : ILoginViewModel
        {
            container.RegisterInstance<ILoginViewModel>(loginViewModel);
            loginViewModel.Initialize()
                .ContinueWith(lt =>
                {
                    var loggedIn = false;
                    var loginView = loginViewModel.GetView() as Window;

                    if (loginView == null)
                        throw new ApplicationBuildException("The login view muse be of type Window");

                    loginView.Closed += delegate
                    {
                        if (!loggedIn)
                            wpfApplication.GetSystemApplication().Shutdown();
                    };
                    loginViewModel.LoggedIn += (s, args) =>
                    {
                        loggedIn = true;
                        shellViewModel.Initialize().ContinueWith((t) =>
                        {
                            container.Resolve<IEventDispatcher>().GetEvent<ApplicationStartedEvent>().Raise(wpfApplication);
                        });
                        wpfApplication.ShowWindow();
                        loginView.Close();
                    };
                    wpfApplication.GetSystemApplication()
                        .Dispatcher.Invoke(() =>
                        {
                            loginView.Show();
                        });
                });
        }

        private static TShellViewModel BuildCustomShell<TShellViewModel>(Action<Window> config,
                                                                         IContainer container)
            where TShellViewModel : IShellViewModel
        {
            container.Resolve<IGlobalVariablesService>().GetVariable<UsingDefaultShellGlobalVariable>().SetValue(false);
            var vmManager = container.Resolve<IViewModelsManager>();

            var shellViewModel = vmManager.CreateViewModel<TShellViewModel>(null);
            container.RegisterInstance<IShellViewModel>(shellViewModel);

            var wpfApplication = container.Resolve<WPFApplication>();
            var systemApplication = wpfApplication.GetSystemApplication();

            var shell = (IShell)shellViewModel.GetView();
            container.RegisterInstance<IShell>(shell);

            var shellWindow = (Window)shell;
            config?.Invoke(shellWindow);
            systemApplication.MainWindow = shellWindow;
            shellWindow.Closed += delegate { systemApplication.Shutdown(); };
            return shellViewModel;
        }

        private static IDefaultShellViewModel BuildDefaultShell<TContent>(IApplicationBuilder builder, WindowState state, Action<Window> config, IContainer container) where TContent : IPresentable
        {
            container.Resolve<IGlobalVariablesService>().GetVariable<UsingDefaultShellGlobalVariable>().SetValue(true);

            var vmManager = container.Resolve<IViewModelsManager>();
            var shellViewModel = vmManager.CreateViewModel<DefaultShellViewModel>(null);

            container.RegisterInstance<IShellViewModel>(shellViewModel);
            container.RegisterInstance<IDefaultShellViewModel>(shellViewModel);


            var wpfApplication = container.Resolve<WPFApplication>();
            var systemApplication = wpfApplication.GetSystemApplication();

            var shell = (DefaultShellView)shellViewModel.GetView();
            shell.SetState(state);
            config?.Invoke(shell);
            container.RegisterInstance<IShell>(shell);
            systemApplication.MainWindow = shell;
            shell.Closed += delegate { systemApplication.Shutdown(); };

            shellViewModel.SetMainContent(typeof(TContent),
                                          builder.BuildActionsCoordinator
                                              .BuildActionExists(BuildActionsKeys
                                                                     .Navigation));


            return shellViewModel;
        }

        /// <summary>
        /// Sets a default shell for the WPF application
        /// </summary>
        /// <returns></returns>
        public static IApplicationBuilder UseNavigation(
            this IApplicationBuilder builder,
            params INavigationItem[] navigationItems)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.NavigationBuildAction((container) =>
            {
                var navs = new List<INavigationItem>();
                var nav = container.Resolve<INavigationService>();
                var gud = container.Resolve<IGlobalVariablesService>().GetVariable<UsingDefaultShellGlobalVariable>();
                if (gud.IsValueSet && gud.GetValue())
                {
                    var homeNav = new NavigationItem(container.Resolve<IDefaultShellViewModel>().GetMainContent(),
                                                     "Home",
                                                     "M10,20V14H14V20H19V12H22L12,3L2,12H5V20H10Z");
                    DefaultNavigations.Home = homeNav;

                    navs.Add(homeNav);
                    navs.AddRange(navigationItems);

                    nav.Initialize(DefaultRegions.NavbarRegion,
                                   DefaultRegions.ApplicationMainRegion,
                                   navs);
                }
                else
                {
                    navs.AddRange(navigationItems);
                    nav.Initialize(DefaultRegions.NavbarRegion,
                                   DefaultRegions.ApplicationMainRegion,
                                   navs);
                }
            }));
            return builder;
        }

        /// <summary>
        /// Use Dialog service
        /// </summary>
        /// <returns></returns>
        public static IApplicationBuilder UseDialogs(this IApplicationBuilder builder,
                                                        Region dialogRegion,
                                                        SolidColorBrush dialogBorderBrush)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.DialogServiceBuildAction(
                                                                                                   (container) =>
                     {
                         container.Resolve<IDialogService>().Initialize(dialogRegion, dialogBorderBrush);
                     }));
            return builder;
        }

        /// <summary>
        /// Use Dialog service
        /// </summary>
        /// <returns></returns>
        public static IApplicationBuilder UseDialogs(this IApplicationBuilder builder)
        {
            return builder.UseDialogs(DefaultRegions.DialogRegion, new SolidColorBrush(Colors.WhiteSmoke));
        }

        /// <summary>
        /// Use the toast service
        /// </summary>
        /// <param name="duration">The duration the toast will stay visible in milliseconds</param>
        /// <param name="builder">The application builder</param>
        /// <returns></returns>
        public static IApplicationBuilder UseToast(this IApplicationBuilder builder, double duration = 3000)
        {
            return builder.UseToast(DefaultRegions.ToastRegion, duration);
        }

        /// <summary>
        /// Use the toast service
        /// </summary>
        /// <param name="builder">The application builder</param>
        /// <param name="toastRegion">The region in which the toasts will be viewed</param>
        /// <param name="duration">The duration the toast will stay visible in milliseconds</param>
        /// <returns></returns>
        public static IApplicationBuilder UseToast(this IApplicationBuilder builder,
                                                      Region toastRegion,
                                                      double duration = 3000)
        {
            builder.BuildActionsCoordinator.AddAction(new BuildAction(BuildActionsKeys.Toast, container =>
            {
                container.Resolve<IToastService>().Initialize(toastRegion, TimeSpan.FromMilliseconds(duration));
            }));
            return builder;
        }

        public static IApplicationBuilder UseNotification(this IApplicationBuilder builder,Region notificationRegion,double duration)
        {
            builder.BuildActionsCoordinator.AddActionAfter(new BuildAction(BuildActionsKeys.Notification,
                                                                           container =>
                                                                           {
                                                                               NotificationService service= (NotificationService)container.Resolve<INotificationService>();
                                                                               service.Inititlize(notificationRegion,duration);
                                                                           }), BuildActionsKeys.Toast);
            return builder;
        }
        public static IApplicationBuilder UseNotification(this IApplicationBuilder builder)
        {
            return builder.UseNotification(DefaultRegions.NotificationRegion, 5000);
        }
    }

}
