// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Coddee.AppBuilder;
using Coddee.Loggers;
using Coddee.Unity;
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
            var dbLocation = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB"));
            var connection = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={dbLocation}\HRDatabase.mdf;Integrated Security=True;Connect Timeout=30";

            new WPFApplication("HR application", new CoddeeUnityContainer()).Run(app =>
            {
                app.UseLogger(LoggerTypes.ApplicationConsole | LoggerTypes.DebugOutput, LogRecordTypes.Debug)
                   .UseApplicationConsole(e => e.Key == Key.F12)
                   .UseCoddeeDebugTool(e => e.Key == Key.F11)
                   .UseILMapper()
                   .UseDefaultShellWithLogin<MainViewModel, LoginViewModel>()
                   .UseNavigation(HRNavigation.Navigations)
                   .UseToast()
                   .UseDialogs()
                   .UseLocalization("HR.Clients.WPF.Properties.Resources", "HR.Clients.WPF.exe", new[] { "ar-SY", "en-US" }, "ar-SY")
                   //.UseRESTRepositoryManager(config =>
                   //{
                   //    return new RESTRepositoryManagerConfig("http://localhost:15297/dapi/", null,"HR.Data.REST");
                   //});
                 .UseLinqRepositoryManager<HRDBManager>(connection, "HR.Data.LinqToSQL");
             }, args);
            base.OnStartup(args);
        }
    }
}