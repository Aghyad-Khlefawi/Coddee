// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Coddee.AppBuilder;
using Coddee.Data;
using Coddee.Loggers;
using Coddee.Unity;
using Coddee.WPF;
using Coddee.WPF.AppBuilder;
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
        protected override void OnStartup(StartupEventArgs e)
        {
            new WPFApp().Run(new CoddeeUnityContainer());
            base.OnStartup(e);
        }
    }


    public class WPFApp : WPFApplication
    {
        public static Guid AppID = new Guid("c1fae5da-5d56-4116-b574-82609a453ee0");

        public override void BuildApplication(IWPFApplicationFactory app)
        {
            var config = new RepositoryConfigurations();
            var dbLocation = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\", "..\\", "..\\", "HR.Web", "DB"));

            var connection = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={dbLocation}\HRDatabase.mdf;Integrated Security=True;Connect Timeout=30";


            app.CreateWPFApplication("HR application", AppID)
                .UseLogger(LoggerTypes.ApplicationConsole | LoggerTypes.DebugOutput, LogRecordTypes.Debug)
                .UseApplicationConsole(e => e.Key == Key.F12)
                .UseCoddeeDebugTool(e => e.Key == Key.F11)
                .UseILMapper()
                .UseDefaultShellWithLogin<MainViewModel, LoginViewModel>()
                .UseNavigation(HRNavigation.Navigations)
                .UseToast()
                .UseDialogs()
                .UseLocalization("HR.Clients.WPF.Properties.Resources",
                                 "HR.Clients.WPF.exe",
                                 new[] { "ar-SY", "en-US" },
                                 "ar-SY")
                .UseLinqRepositoryManager<HRDBManager, HRRepositoryManager>(connection,
                  "HR.Data.LinqToSQL",true,config);
        }
    }
}