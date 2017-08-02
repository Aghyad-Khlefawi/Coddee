// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Media;
using Coddee.AppBuilder;
using Coddee.Loggers;
using Coddee.Services;
using Coddee.Windows.Mapper;
using Coddee.WPF.DefaultShell;
using Coddee.WPF.Events;
using Coddee.WPF.Modules;
using Coddee.WPF.Modules.Dialogs;
using Coddee.WPF.Navigation;
using Coddee.WPF.Security;
using Microsoft.Practices.Unity;

namespace Coddee.WPF.AppBuilder
{
    public static class BuilderExtensions
    {
        public static IWPFApplicationBuilder UseLocalization(
            this IWPFApplicationBuilder builder,
            string defaultCluture = "en-US")
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.LocalizationBuildAction(
                                                                                                  (container) =>
                                                                                                  {
                                                                                                      var localizationManager = container.Resolve<ILocalizationManager>();
                                                                                                      localizationManager.SetCulture(defaultCluture);
                                                                                                      container.RegisterInstance<IObjectMapper, ILObjectsMapper>();
                                                                                                  }));
            return builder;
        }

        public static IWPFApplicationBuilder UseLocalization(
            this IWPFApplicationBuilder builder,
            string resourceManagerFullPath,
            string resourceManagerAssembly,
            string[] supportedCultures,
            string defaultCluture = "en-US")
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.LocalizationBuildAction((container) =>
            {
                var localizationManager = container.Resolve<ILocalizationManager>();
                localizationManager.SetCulture(defaultCluture);
                var values = new Dictionary<string, Dictionary<string, string>>();
                var res = new ResourceManager(resourceManagerFullPath,
                                              Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain
                                                                                 .BaseDirectory,
                                                                             resourceManagerAssembly)));
                foreach (var culture in supportedCultures)
                {
                    foreach (DictionaryEntry val in
                        res.GetResourceSet(new CultureInfo(culture), true, true))
                    {
                        if (!values.ContainsKey(val.Key.ToString()))
                            values[val.Key.ToString()] = new Dictionary<string, string>();
                        values[val.Key.ToString()][culture] = val.Value.ToString();
                    }
                }
                localizationManager.AddValues(values);
            }));
            return builder;
        }

        /// <summary>
        /// Use the IL object mapper
        /// </summary>
        public static IWPFApplicationBuilder UseILMapper(this IWPFApplicationBuilder builder)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.MapperBuildAction(
                                                                                            (container) =>
                                                                                            {
                                                                                                container.RegisterInstance<IObjectMapper, ILObjectsMapper>();
                                                                                            }));
            return builder;
        }

        /// <summary>
        /// Use the basic object mapper
        /// </summary>
        public static IWPFApplicationBuilder UseBasicMapper(this IWPFApplicationBuilder builder)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.MapperBuildAction(
                                                                                            (container) =>
                                                                                            {
                                                                                                container.RegisterInstance<IObjectMapper, BasicObjectMapper>();
                                                                                            }));
            return builder;
        }


        /// <summary>
        /// Initialize the configuration manager
        /// </summary>
        /// <param name="encryptFile">If set to true the configuration manager will use encrypted configuration file</param>
        /// <param name="configFile">The file path without the extension</param>
        public static IWPFApplicationBuilder UseConfigurationFile(
            this IWPFApplicationBuilder builder,
            bool encryptFile = false,
            string configFile = "config",
            Dictionary<string, object> defaultValues = null)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.ConfigFileBuildAction(
                                                                                                (container) =>
                                                                                                {
                                                                                                    var config = container.Resolve<IConfigurationManager>();
                                                                                                    config.Initialize(configFile, defaultValues);
                                                                                                    if (encryptFile)
                                                                                                        config.SetEncrpytion(builder.WPFBuilder.GetApp().ApplicationID.ToString());
                                                                                                    config.ReadFile();
                                                                                                }));
            return builder;
        }

        /// <summary>
        /// Add a console to the application
        /// </summary>
        /// <param name="toggleCondition">A function that is executed on the shell KeyDown event show return true to toggle the console</param>
        /// <returns></returns>
        public static IWPFApplicationBuilder UseApplicationConsole(this IWPFApplicationBuilder builder,
                                                                   Func<KeyEventArgs, bool> toggleCondition)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.AppConsoleBuildAction(
                                                                                                (container) =>
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
                                                                                                    //when Ctrl+F12 are pressed
                                                                                                    applicationConsole.SetToggleCondition(toggleCondition);
                                                                                                }));
            return builder;
        }

        /// <summary>
        /// Add a console to the application
        /// </summary>
        /// <param name="toggleCondition">A function that is executed on the shell KeyDown event show return true to toggle the console</param>
        /// <returns></returns>
        public static IWPFApplicationBuilder UseCoddeeDebugTool(this IWPFApplicationBuilder builder,
                                                                Func<KeyEventArgs, bool> toggleCondition)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.DebugToolBuildAction(
                                                                                               async (container) =>
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
        /// Sets the minimum log level to show to the user
        /// </summary>
        /// <param name="loggerType">Specify which logger to use. Uses Enum flags to specify multiple values</param>
        /// <param name="level">The minimum log level</param>
        /// <returns></returns>
        public static IWPFApplicationBuilder UseLogger(this IWPFApplicationBuilder builder,
                                                       LoggerTypes loggerType,
                                                       LogRecordTypes level)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.LoggerBuildAction(
                                                                                            (container) =>
                                                                                            {
                                                                                                var logger = (LogAggregator)container.Resolve<ILogger>();
                                                                                                logger.SetLogLevel(level);
                                                                                                logger.AllowedTypes = loggerType;

                                                                                                if (loggerType.HasFlag(LoggerTypes.DebugOutput))
                                                                                                {
                                                                                                    var debugLogger = container.Resolve<DebugOuputLogger>();
                                                                                                    debugLogger.Initialize(level);
                                                                                                    logger.AddLogger(debugLogger, LoggerTypes.DebugOutput);
                                                                                                }
                                                                                                if (loggerType.HasFlag(LoggerTypes.File))
                                                                                                {
                                                                                                    var fileLogger = container.Resolve<FileLogger>();
                                                                                                    fileLogger.Initialize(level, "log.txt");
                                                                                                    logger.AddLogger(fileLogger, LoggerTypes.File);
                                                                                                }
                                                                                            }));
            return builder;
        }

        /// <summary>
        /// Sets a default shell for the WPF application
        /// </summary>
        /// <typeparam name="TContent">The main content type to be shown on startup</typeparam>
        /// <returns></returns>
        public static IWPFApplicationBuilder UseDefaultShell<TContent>(
            this IWPFApplicationBuilder builder,
            WindowState state = WindowState.Maximized,
            Action<Window> config = null)
            where TContent : IPresentable
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.ShellBuildAction((container) =>
            {
                var wpfApplication = builder.WPFBuilder.GetApp();
                var shellViewModel = BuildDefaultShell<TContent>(builder, state, config, container);
                shellViewModel.Initialize().ContinueWith((t) =>
                {
                    container.Resolve<IGlobalEventsService>().GetEvent<ApplicationStartedEvent>().Invoke(wpfApplication);
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
        public static IWPFApplicationBuilder UseDefaultShellWithLogin<TContent, TLogin>(
            this IWPFApplicationBuilder builder,
            WindowState state = WindowState.Maximized,
            Action<Window> config = null)
            where TContent : IPresentable
            where TLogin : ILoginViewModel
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.ShellBuildAction((container) =>
            {
                var wpfApplication = builder.WPFBuilder.GetApp();
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
        public static IWPFApplicationBuilder UseCustomShell<TShellViewModel>(this IWPFApplicationBuilder builder,
                                                                             Action<Window> config = null)
            where TShellViewModel : IShellViewModel
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.ShellBuildAction((container) =>
            {
                var wpfApplication = builder.WPFBuilder.GetApp();
                var shellViewModel = BuildCustomShell<TShellViewModel>(builder, config, container);
                shellViewModel.Initialize().ContinueWith((t) =>
                {
                    container.Resolve<IGlobalEventsService>().GetEvent<ApplicationStartedEvent>().Invoke(wpfApplication);
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
        public static IWPFApplicationBuilder UseCustomShellWithLogin<TShellViewModel, TLogin>(this IWPFApplicationBuilder builder,
                                                                                              Action<Window> config = null)
            where TShellViewModel : IShellViewModel
            where TLogin : ILoginViewModel
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.ShellBuildAction((container) =>
            {
                var wpfApplication = builder.WPFBuilder.GetApp();
                var shellViewModel = BuildCustomShell<TShellViewModel>(builder, config, container);
                var loginViewModel = container.Resolve<TLogin>();
                BuildLogin(container, wpfApplication, shellViewModel, loginViewModel);
            }));
            return builder;
        }

        private static void BuildLogin<TShellViewModel, TLogin>(IUnityContainer container, WPFApplication wpfApplication, TShellViewModel shellViewModel, TLogin loginViewModel)
            where TShellViewModel : IShellViewModel
            where TLogin : ILoginViewModel
        {
            loginViewModel.Initialize()
                .ContinueWith(lt =>
                {
                    var loginView = loginViewModel.GetView() as Window;
                    if (loginView == null)
                        throw new ApplicationBuildException("The login view muse be of type Window");
                    loginViewModel.LoggedIn += (s, args) =>
                    {
                        shellViewModel.Initialize().ContinueWith((t) =>
                        {
                            container.Resolve<IGlobalEventsService>().GetEvent<ApplicationStartedEvent>().Invoke(wpfApplication);
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

        private static TShellViewModel BuildCustomShell<TShellViewModel>(IWPFApplicationBuilder builder,
                                                                         Action<Window> config,
                                                                         IUnityContainer container)
            where TShellViewModel : IShellViewModel
        {
            container.Resolve<IGlobalVariablesService>().SetValue(Globals.UsingDefaultShell, true);

            var shellViewModel = container.Resolve<TShellViewModel>();
            container.RegisterInstance<IShellViewModel>(shellViewModel);

            var wpfApplication = builder.WPFBuilder.GetApp();
            var systemApplication = wpfApplication.GetSystemApplication();

            var shell = (IShell)shellViewModel.GetView();
            container.RegisterInstance<IShell>(shell);

            var shellWindow = (Window)shell;
            config?.Invoke(shellWindow);
            systemApplication.MainWindow = shellWindow;
            shellWindow.Closed += delegate { systemApplication.Shutdown(); };
            return shellViewModel;
        }

        private static IDefaultShellViewModel BuildDefaultShell<TContent>(IWPFApplicationBuilder builder, WindowState state, Action<Window> config, IUnityContainer container) where TContent : IPresentable
        {
            container.Resolve<IGlobalVariablesService>().SetValue(Globals.UsingDefaultShell, true);
            container.RegisterInstance<IShellViewModel, DefaultShellViewModel>();
            container.RegisterInstance<IDefaultShellViewModel, DefaultShellViewModel>();


            var wpfApplication = builder.WPFBuilder.GetApp();
            var systemApplication = wpfApplication.GetSystemApplication();

            var shellViewModel = container.Resolve<IDefaultShellViewModel>();
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
        public static IWPFApplicationBuilder UseNavigation(
            this IWPFApplicationBuilder builder,
            params INavigationItem[] navigationItems)
        {
            builder.BuildActionsCoordinator.AddAction(DefaultBuildActions.NavigationBuildAction((container) =>
            {
                var navs = new List<INavigationItem>();
                var nav = container.Resolve<INavigationService>();
                if (container.Resolve<IGlobalVariablesService>().TryGetValue(Globals.UsingDefaultShell, out bool usingDefault) && usingDefault)
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
        public static IWPFApplicationBuilder UseDialogs(this IWPFApplicationBuilder builder,
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
        public static IWPFApplicationBuilder UseDialogs(this IWPFApplicationBuilder builder)
        {
            return builder.UseDialogs(DefaultRegions.DialogRegion, new SolidColorBrush(Colors.WhiteSmoke));
        }

        /// <summary>
        /// Use the toast service
        /// </summary>
        /// <param name="duration">The duration the toast will stay visible in milliseconds</param>
        /// <param name="builder">The application builder</param>
        /// <returns></returns>
        public static IWPFApplicationBuilder UseToast(this IWPFApplicationBuilder builder, double duration = 3000)
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
        public static IWPFApplicationBuilder UseToast(this IWPFApplicationBuilder builder,
                                                      Region toastRegion,
                                                      double duration = 3000)
        {
            builder.BuildActionsCoordinator.AddAction(new BuildAction(BuildActionsKeys.Toast, container =>
            {
                container.Resolve<IToastService>().Initialize(toastRegion, duration);
            }));
            return builder;
        }
    }

}
