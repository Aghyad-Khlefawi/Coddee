// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Practices.Unity;
using Coddee.Loggers;
using Coddee.WPF.AppBuilder;
using Coddee.AppBuilder;
using Coddee.Data;
using Coddee.ModuleDefinitions;
using Coddee.Services;
using Coddee.SQL;
using Coddee.Windows.Mapper;
using Coddee.WPF.DefaultShell;
using Coddee.WPF.Modules;
using Coddee.WPF.Modules.Dialogs;
using Coddee.WPF.Navigation;
using Coddee.WPF.Security;

namespace Coddee.WPF
{
    public interface IWPFApplicationBuilder : IApplicationBuilder
    {
        WPFApplicationBuilder WPFBuilder { get; }
    }

    public class WPFApplicationBuilder : IWPFApplicationFactory, IWPFApplicationBuilder
    {
        private const string EventsSource = "WPFApplicationBuilder";

        private readonly WPFApplication _app;
        private Application _systemApplication => _app.GetSystemApplication();
        private readonly IUnityContainer _container;
        private readonly LogAggregator _logger;
        private IApplicationModulesManager _modulesManager;

        private bool _usingDefaultShell;
        private Type _defaultPresentable;
        private IViewModel _mainContent;
        private IShell _shell;
        private LoggerTypes _loggerType;
        private ILoginViewModel _loginViewModel;

        public WPFApplicationBuilder(WPFApplication app, IUnityContainer container)
        {
            _app = app;
            _container = container;
            _logger = _container.Resolve<LogAggregator>();
            _container.RegisterInstance<ILogger>(_logger);
            _buildActions = new Dictionary<string, Action>();
        }


        //Build Actions
        private readonly Dictionary<string, Action> _buildActions;
        private LogRecordTypes _logLevel;


        public void SetBuildAction(string actionName, Action action)
        {
            _buildActions[actionName] = action;
        }

        public Action GetBuildAction(string actionName)
        {
            return _buildActions.ContainsKey(actionName) ? _buildActions[actionName] : null;
        }

        void InvokeBuildAction(string actionName)
        {
            GetBuildAction(actionName)?.Invoke();
        }

        /// <summary>
        /// Returns the application builder
        /// </summary>
        /// <param name="applicationName">The application name</param>
        /// <param name="applicationID">A GUID to globally identify the application</param>
        /// <returns></returns>
        public IWPFApplicationBuilder CreateWPFApplication(string applicationName, Guid applicationID)
        {
            _app.SetApplicationName(applicationName);
            _app.SetApplicationID(applicationID);
            _app.SetApplicationType(ApplicationTypes.WPF);

            _logger.Log(EventsSource,
                        $"using Coddee.Core: {FileVersionInfo.GetVersionInfo(Assembly.Load("Coddee.Core").Location).ProductVersion}");
            _logger.Log(EventsSource,
                        $"using Coddee.Data: {FileVersionInfo.GetVersionInfo(Assembly.Load("Coddee.Data").Location).ProductVersion}");
            _logger.Log(EventsSource,
                        $"using Coddee.WPF: {FileVersionInfo.GetVersionInfo(Assembly.Load("Coddee.WPF").Location).ProductVersion}");

            _modulesManager = _container.Resolve<ApplicationModulesManager>();


            _modulesManager.RegisterModule(CoreModuleDefinitions.Modules);
            _modulesManager.RegisterModule(WindowsModuleDefinitions.Modules);
            _modulesManager.RegisterModule(WPFModuleDefinitions.Modules);

            _modulesManager.InitializeAutoModules();
            _app.OnAutoModulesInitialized();
            _container.Resolve<IGlobalVariablesService>().SetValue(Globals.ApplicationName, applicationName);
            return this;
        }


        /// <summary>
        /// Starts the application by setting the SystemApplication MainWindow and Call the RunApplication method in 
        /// WPFApplication class
        /// </summary>
        public void Start()
        {
            _systemApplication.Startup += OnStartup;
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            _logger.Log(EventsSource, "Application startup.");

            _systemApplication.Dispatcher.Invoke(() =>
            {
                UISynchronizationContext
                    .SetContext(SynchronizationContext.Current);
            });
            ViewModelBase.SetApplication(_app);
            ViewModelBase.SetContainer(_container);

            try
            {
                //Invoke startup sequence
                _logger.Log(EventsSource, "Initiating startup sequence");

                InvokeBuildAction(BuildActions.Logger);
                InvokeBuildAction(BuildActions.Localization);
                InvokeBuildAction(BuildActions.Mapper);
                InvokeBuildAction(BuildActions.ConfigFile);
                InvokeBuildAction(BuildActions.Repository);
                InvokeBuildAction(BuildActions.Login);
                InvokeBuildAction(BuildActions.Shell);
                InvokeBuildAction(BuildActions.AppConsole);
                InvokeBuildAction(BuildActions.DebugTool);
                InvokeBuildAction(BuildActions.DialogService);


                if (GetBuildAction(BuildActions.Toast) == null)
                    this.UseToast();
                InvokeBuildAction(BuildActions.Toast);


                //If using login
                if (_loginViewModel != null)
                {
                    await _loginViewModel.Initialize();
                    var loginView = _loginViewModel.GetView() as Window;
                    if (loginView == null)
                        throw new ApplicationBuildException("The login view muse be of type Window");
                    _loginViewModel.LoggedIn += (s, args) =>
                    {
                        ShowMain();
                        loginView.Close();
                    };
                    loginView.Show();
                }
                else
                    ShowMain();
            }
            catch (Exception ex)
            {
                _logger.Log(EventsSource, ex);
                throw;
            }
            _logger.Log(EventsSource, $"Application started", LogRecordTypes.Information);
        }

        /// <summary>
        /// Initialize and view the first window
        /// </summary>
        private async void ShowMain()
        {
            {
                //Show the application window
                _systemApplication.MainWindow = (Window) _shell;
                _systemApplication.MainWindow.Closed += delegate { _systemApplication.Shutdown(); };
                _app.ShowWindow();

                //Initialize shell view model
                var shellViewModel = _container.Resolve<IShellViewModel>();
                var shellVmBase = shellViewModel as ViewModelBase;
                if (_usingDefaultShell)
                {
                    await ((IDefaultShellViewModel) shellVmBase).Initialize();
                    _mainContent = ((IDefaultShellViewModel) shellVmBase).SetMainContent(_defaultPresentable,
                                                                                         _buildActions
                                                                                             .ContainsKey(BuildActions
                                                                                                              .Navigation));
                }
                else
                    await shellVmBase?.Initialize();

                InvokeBuildAction(BuildActions.Navigation);

                var shell = _container.Resolve<IShell>();
                ((Window) shell).DataContext = shellViewModel;
            }
            ;
        }

        /// <summary>
        /// Creates a default shell if no shell was provided at the application build phase
        /// </summary>
        /// <returns></returns>
        private IShell CreateDefaultShell()
        {
            _logger.Log(EventsSource, $"Creating default shell", LogRecordTypes.Debug);
            return _container.Resolve<DefaultShellView>();
        }

        /// <summary>
        /// Return the dependency injection container
        /// </summary>
        /// <returns></returns>
        public IUnityContainer GetContainer()
        {
            return _container;
        }

        /// <summary>
        /// Returns the application shell object
        /// </summary>
        /// <returns></returns>
        public IShell GetShell()
        {
            return _shell;
        }

        /// <summary>
        /// Register the default shell as the application shell
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        public Window SetDefaultShell<TContent>(WindowState state) where TContent : IPresentable
        {
            _shell = CreateDefaultShell();
            ((DefaultShellView) _shell).SetState(state);
            _container.RegisterInstance<IShell>(_shell);
            _container.RegisterInstance<IShellViewModel, DefaultShellViewModel>();
            _container.RegisterInstance<IDefaultShellViewModel, DefaultShellViewModel>();
            _defaultPresentable = typeof(TContent);
            _usingDefaultShell = true;
            return (Window) _shell;
        }

        /// <summary>
        /// Add a logger to the aggregate logger
        /// </summary>
        /// <param name="loggerType"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public WPFApplicationBuilder AddLogger(LoggerTypes loggerType, ILogger logger)
        {
            if (_loggerType.HasFlag(loggerType))
            {
                _logger.AddLogger(logger);
            }
            return this;
        }


        /// <summary>
        /// Configure and required dependency injection
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public WPFApplicationBuilder ConfigureContainer(Action<IUnityContainer> config)
        {
            config(_container);
            return this;
        }

        /// <summary>
        /// Register the repository manager in the container
        /// </summary>
        public void SetRepositoryManager(IRepositoryManager repositoryManager, bool registerTheRepositoresInContainer)
        {
            _logger.Log(EventsSource,
                        $"Registering repository manager of type {repositoryManager.GetType().Name}",
                        LogRecordTypes.Debug);
            _container.RegisterInstance<IRepositoryManager>(repositoryManager);
            _app.OnRepositoryManagerSet();
            if (registerTheRepositoresInContainer)
                foreach (var repository in repositoryManager.GetRepositories())
                {
                    _logger.Log(EventsSource,
                                $"Registering repository of type {repository.GetType().Name}",
                                LogRecordTypes.Debug);
                    _container.RegisterInstance(repository.ImplementedInterface, repository);
                }
        }

        /// <summary>
        /// Returns the registered IObjectsMapper
        /// </summary>
        /// <returns></returns>
        public IObjectMapper GetMapper()
        {
            return _container.Resolve<IObjectMapper>();
        }


        /// <summary>
        /// Uses SQLDBBrowser to let the use select an SQL Server database
        /// </summary>
        /// <returns></returns>
        public string GetSQLDBConnection()
        {
            if (!_container.IsRegistered<IConfigurationManager>())
            {
                this.UseConfigurationFile(true);
                InvokeBuildAction(BuildActions.ConfigFile);
            }
            var config = _container.Resolve<IConfigurationManager>();
            string connection = null;
            if (config.TryGetValue(BuiltInConfigurationKeys.SQLDBConnection, out connection) &&
                !string.IsNullOrEmpty(connection))
            {
                return connection;
            }
            var browser = _container.Resolve<ISQLDBBrowser>();
            connection = browser.GetDatabaseConnectionString();
            config.SetValue(BuiltInConfigurationKeys.SQLDBConnection, connection);
            return connection;
        }

        /// <summary>
        /// Returns the application builder as a WPFApplicationBuilder
        /// </summary>
        /// <value></value>
        public WPFApplicationBuilder WPFBuilder => this;


        /// <summary>
        /// Returns the WPFApplication instance
        /// </summary>
        /// <returns></returns>
        public WPFApplication GetApp()
        {
            return _app;
        }


        public void SetShell(IShell shell)
        {
            _shell = shell;
            _container.RegisterInstance<IShell>(shell);
            var window = shell as Window;
            _app.GetSystemApplication().MainWindow =
                window ?? throw new ApplicationBuildException("The application shell must be a Window");
        }

        public ILogger GetLogger()
        {
            return _logger;
        }

        public void SetLogType(LoggerTypes loggerType)
        {
            _loggerType = loggerType;
        }

        public IPresentable GetDefaultPresentable()
        {
            return (IPresentable) _mainContent;
        }

        public void SetLoginViewModel(ILoginViewModel vm)
        {
            _loginViewModel = vm;
        }

        public void SetLogLevel(LogRecordTypes level)
        {
            _logLevel = level;
        }

        public LogRecordTypes GetLogLevel()
        {
            return _logLevel;
        }
    }

    public static class BuilderExtensions
    {
        public static IWPFApplicationBuilder UseLocalization(
            this IWPFApplicationBuilder builder,
            string defaultCluture = "en-US")
        {
            builder.SetBuildAction(BuildActions.Localization,
                                   () =>
                                   {
                                       var localizationManager = builder.GetContainer().Resolve<ILocalizationManager>();
                                       localizationManager.SetCulture(defaultCluture);
                                       builder.GetContainer().RegisterInstance<IObjectMapper, ILObjectsMapper>();
                                   });
            return builder;
        }

        public static IWPFApplicationBuilder UseLocalization(
            this IWPFApplicationBuilder builder,
            string resourceManagerFullPath,
            string resourceManagerAssembly,
            string[] supportedCultures,
            string defaultCluture = "en-US")
        {
            Action action = () =>
            {
                var localizationManager = builder.GetContainer().Resolve<ILocalizationManager>();
                localizationManager.SetCulture(defaultCluture);
                var values = new Dictionary<string, Dictionary<string, string>>();
                var res =
                    new ResourceManager(resourceManagerFullPath,
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
            };
            builder.SetBuildAction(BuildActions.Localization,
                                   action);
            return builder;
        }

        /// <summary>
        /// Use the IL object mapper
        /// </summary>
        public static IWPFApplicationBuilder UseILMapper(this IWPFApplicationBuilder builder)
        {
            builder.SetBuildAction(BuildActions.Mapper,
                                   () =>
                                   {
                                       builder.GetContainer().RegisterInstance<IObjectMapper, ILObjectsMapper>();
                                   });
            return builder;
        }

        /// <summary>
        /// Use the basic object mapper
        /// </summary>
        public static IWPFApplicationBuilder UseBasicMapper(this IWPFApplicationBuilder builder)
        {
            builder.SetBuildAction(BuildActions.Mapper,
                                   () =>
                                   {
                                       builder.GetContainer().RegisterInstance<IObjectMapper, BasicObjectMapper>();
                                   });
            return builder;
        }

        /// <summary>
        /// Sets the login handler, the will cause a login to be required before showing the application shell
        /// </summary>
        public static IWPFApplicationBuilder UseLogin<TLoginViewModel>(this IWPFApplicationBuilder builder)
            where TLoginViewModel : ILoginViewModel
        {
            builder.SetBuildAction(BuildActions.Login,
                                   () =>
                                   {
                                       builder.WPFBuilder.SetLoginViewModel(builder.GetContainer()
                                                                                .Resolve<TLoginViewModel>());
                                   });
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
            builder.SetBuildAction(BuildActions.ConfigFile,
                                   delegate
                                   {
                                       var config = builder.WPFBuilder.GetContainer().Resolve<IConfigurationManager>();
                                       config.Initialize(configFile, defaultValues);
                                       if (encryptFile)
                                           config.SetEncrpytion(builder.WPFBuilder.GetApp().ApplicationID.ToString());
                                       config.ReadFile();
                                   });
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
            var appBuilder = builder.WPFBuilder;
            appBuilder.SetBuildAction(BuildActions.AppConsole,
                                      delegate
                                      {
                                          Window shell = (Window) appBuilder.GetShell();
                                          if (shell == null)
                                              throw new
                                                  ApplicationBuildException("The method must be called after the UseShell method");
                                          var applicationConsole =
                                              appBuilder.GetContainer().Resolve<IApplicationConsole>();
                                          applicationConsole.Initialize(appBuilder.GetShell(),
                                                                        appBuilder.GetLogLevel());
                                          //Add the console logger to loggers collection
                                          appBuilder.AddLogger(LoggerTypes.ApplicationConsole,
                                                               applicationConsole.GetLogger());

                                          //Sets the Shell KeyDown event handler to toggle the console visibility
                                          //when Ctrl+F12 are pressed
                                          applicationConsole.SetToggleCondition(toggleCondition);
                                      });
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
            var appBuilder = builder.WPFBuilder;
            appBuilder.SetBuildAction(BuildActions.DebugTool,
                                      async () =>
                                      {
                                          Window shell = (Window) appBuilder.GetShell();
                                          if (shell == null)
                                              throw new
                                                  ApplicationBuildException("The method must be called after the UseShell method");
                                          var debugTool =
                                              appBuilder.GetContainer().Resolve<IDebugTool>();

                                          await debugTool.Initialize();

                                          debugTool.SetToggleCondition(toggleCondition);
                                      });
            return builder;
        }

        /// <summary>
        /// Sets a custom shell for the WPF application
        /// </summary>
        /// <typeparam name="TShell"></typeparam>
        /// <typeparam name="TShellViewModel"></typeparam>
        /// <returns></returns>
        public static IWPFApplicationBuilder UseShell<TShell, TShellViewModel>(this IWPFApplicationBuilder builder)
            where TShell : Window, IShell
            where TShellViewModel : IShellViewModel
        {
            var appBuilder = builder.WPFBuilder;
            appBuilder.SetBuildAction(BuildActions.Shell,
                                      delegate
                                      {
                                          appBuilder.GetContainer()
                                              .RegisterInstance<IShellViewModel, TShellViewModel>();
                                          var shell = appBuilder.GetContainer().Resolve<TShell>();
                                          appBuilder.SetShell(shell);
                                      });
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
            var appBuilder = builder.WPFBuilder;
            builder.SetBuildAction(BuildActions.Logger,
                                   delegate
                                   {
                                       appBuilder.WPFBuilder.SetLogLevel(level);

                                       var logger = appBuilder.GetLogger();
                                       logger.Initialize(level);
                                       appBuilder.SetLogType(loggerType);

                                       if (loggerType.HasFlag(LoggerTypes.DebugOutput))
                                       {
                                           var debugLogger = appBuilder.GetContainer().Resolve<DebugOuputLogger>();
                                           debugLogger.Initialize(level);
                                           appBuilder.AddLogger(LoggerTypes.DebugOutput, debugLogger);
                                       }
                                       if (loggerType.HasFlag(LoggerTypes.File))
                                       {
                                           var fileLogger = appBuilder.GetContainer().Resolve<FileLogger>();
                                           fileLogger.Initialize(level, "log.txt");
                                           appBuilder.AddLogger(LoggerTypes.File, fileLogger);
                                       }
                                   });
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
            builder.SetBuildAction(BuildActions.Shell,
                                   () =>
                                   {
                                       var win = builder.WPFBuilder.SetDefaultShell<TContent>(state);
                                       config?.Invoke(win);
                                   });
            return builder;
        }

        /// <summary>
        /// Sets a default shell for the WPF application
        /// </summary>
        /// <returns></returns>
        public static IWPFApplicationBuilder UseNavigation(
            this IWPFApplicationBuilder builder,
            params INavigationItem[] navigationItems)
        {
            builder.SetBuildAction(BuildActions.Navigation,
                                   () =>
                                   {
                                       var container = builder.WPFBuilder.GetContainer();
                                       var navs = new List<INavigationItem>();
                                       var nav = container.Resolve<INavigationService>();
                                       if (builder.WPFBuilder.GetDefaultPresentable() != null)
                                       {
                                           var homeNav = new NavigationItem(builder.WPFBuilder.GetDefaultPresentable(),
                                                                            "Home",
                                                                            "M10,20V14H14V20H19V12H22L12,3L2,12H5V20H10Z")
                                           {
                                               IsSelected = true,
                                               IsFirstItem = true
                                           };

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
                                   });
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
            builder.SetBuildAction(BuildActions.DialogService,
                                   () =>
                                   {
                                       builder.GetContainer()
                                           .Resolve<IDialogService>()
                                           .Initialize(dialogRegion, dialogBorderBrush);
                                   });
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
        /// <returns></returns>
        public static IWPFApplicationBuilder UseToast(this IWPFApplicationBuilder builder, double duration = 3000)
        {
            return builder.UseToast(DefaultRegions.ToastRegion, duration);
        }

        /// <summary>
        /// Use the toast service
        /// </summary>
        /// <param name="duration">The duration the toast will stay visible in milliseconds</param>
        /// <returns></returns>
        public static IWPFApplicationBuilder UseToast(this IWPFApplicationBuilder builder,
                                                      Region toastRegion,
                                                      double duration = 3000)
        {
            builder.SetBuildAction(BuildActions.Toast,
                                   delegate
                                   {
                                       builder.WPFBuilder.GetContainer()
                                           .Resolve<IToastService>()
                                           .Initialize(toastRegion, duration);
                                   });
            return builder;
        }
    }
}