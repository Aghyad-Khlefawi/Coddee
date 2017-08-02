// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Coddee.Data;
using Coddee.Loggers;
using Coddee.WPF;
using Coddee.WPF.AppBuilder;
using Coddee.WPF.Navigation;
using HR.Clients.WPF.Login;
using HR.Clients.WPF.Main;
using HR.Clients.WPF.Settings;
using HR.Data.LinqToSQL;

namespace HR.Clients.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            new WPFApp().Run();
            base.OnStartup(e);
        }
    }


    public class WPFApp : WPFApplication
    {
        public static Guid AppID = new Guid("c1fae5da-5d56-4116-b574-82609a453ee0");

        public override void BuildApplication(IWPFApplicationFactory app)
        {
            var config = new RepositoryConfigurations();
            var dbLocation = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"..\\","..\\","..\\","HR.Web","DB"));
            app.CreateWPFApplication("HR application", AppID)
                .UseConfigurationFile(true)
                .UseLogger(LoggerTypes.ApplicationConsole | LoggerTypes.DebugOutput, LogRecordTypes.Debug)
                .UseApplicationConsole(e => e.Key == Key.F12)
                .UseCoddeeDebugTool(e => e.Key == Key.F11)
                .UseILMapper()
                //.UseLogin<LoginViewModel>()
                .UseDefaultShellWithLogin<MainViewModel, LoginViewModel>()
                .UseNavigation(HRNavigation.Navigations)
                .UseToast()
                .UseDialogs()
                .UseLocalization("HR.Clients.WPF.Properties.Resources",
                                 "HR.Clients.WPF.exe",
                                 new[] {"ar-SY", "en-US"},
                                 "ar-SY")
                .UseLinqRepositoryManager<HRDBManager, HRRepositoryManager
                >($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={
                          dbLocation
                      }\HRDatabase.mdf;Integrated Security=True;Connect Timeout=30",
                  "HR.Data.LinqToSQL",
                  true,
                  config);
            //.UseRESTRepositoryManager(config =>
            //{
            //    return new RESTRepositoryManagerConfig{};
            //})
            //.UseMongoDBRepository("mongodb://192.168.1.160:27017", "HR","HR.Data.Mongo",true)
        }

        private void OnUnauthorizedRequest()
        {
            MessageBox.Show("Unauthorized request");
        }
    }
}