// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Coddee;
using Coddee.AppBuilder;
using Coddee.Loggers;
using Coddee.Unity;
using Coddee.Windows.AppBuilder;
using Coddee.WPF;
using HR.Clients.WPF.Login;
using HR.Clients.WPF.Main;
using HR.Data.LinqToSQL;

namespace HR.Clients.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs args)
        {
            //Create the application object
            new WPFApplication("HR application", new CoddeeUnityContainer())
                .Run(app =>
                     {
                         //Set the application features.

                         app

                             //Configure the logger to use the application console, visual studio output window and a text file
                             //with the minimum displayed records of type debug
                             .UseLogger(new LoggerOptions(LoggerTypes.ApplicationConsole | LoggerTypes.DebugOutput | LoggerTypes.File, LogRecordTypes.Debug)
                             {
                                 LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"),
                                 UseFileCompression = true
                             })

                            //Configure the application console to toggle when F12 if pressed
                            .UseApplicationConsole(e => e.Key == Key.F12)

                            //Configure the debug tool to open when F11 is pressed
                            .UseCoddeeDebugTool(e => e.Key == Key.F11)

                            //Use the IL mapper as the IObjectMapper
                            .UseILMapper()

                            //Use the shell created  by the library with a login window
                            .UseDefaultShellWithLogin<MainViewModel, LoginViewModel>()

                            //Configure a navigation bar for the application
                            .UseNavigation(HRNavigation.Navigations)

                            //Use the toast service to display messages to the user.
                            .UseToast()
                            .UseDialogs()

                            //Use localization file for Arabic and English
                            .UseLocalization("HR.Clients.WPF.Properties.Resources", "HR.Clients.WPF.exe", new[] { "ar-SY", "en-US" }, "ar-SY")

                            //Use a singleton repository manager that will keep
                            //using the same instance of the repositories for the 
                            //entire life of the application 
                            .UseTransientRepositoryManager()

                            //Add Rest repositories to the repository manager
                            .UseRESTRepositories(config => new RESTInitializerConfig("http://localhost:15297/dapi/", null, "HR.Data.REST"));

                         //To use linq repositories you can change the last line to:
                         //.UseLinqRepositories<HRDBManager>(new LinqInitializerConfig(GetDbConnection, "HR.Data.LinqToSQL"))

                     },
                     args);

            //Start the application
            base.OnStartup(args);
        }

        string GetDbConnection(IContainer container)
        {
            var dbLocation = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB"));
            return $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={dbLocation}\HRDatabase.mdf;Integrated Security=True;Connect Timeout=30";
        }
    }
}