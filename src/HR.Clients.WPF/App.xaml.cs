// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Coddee;
using Coddee.Collections;
using Coddee.Loggers;
using Coddee.WPF;
using Coddee.WPF.AppBuilder;
using Coddee.WPF.Console;
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
    public partial class App : WPFApplication
    {
        public static Guid AppID = new Guid("c1fae5da-5d56-4116-b574-82609a453ee0");

        public override void BuildApplication(IWPFApplicationFactory app)
        {
            StringBuilder log = new StringBuilder();
            StringLogger logger = new StringLogger();
            logger.Initialize(LogRecordTypes.Debug);
            logger.AppendString += newLog => { log.Append(log); };
            logger.Log("Coddee", "A simple log", LogRecordTypes.Information);


            var dbLocation = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                           "..\\",
                                                           "..\\",
                                                           "..\\",
                                                           "HR.Web",
                                                           "DB"));
            app.CreateWPFApplication("HR application", AppID)
                .UseConfigurationFile(true)
                .UseLogger(LoggerTypes.ApplicationConsole | LoggerTypes.DebugOutput, LogRecordTypes.Debug)
                .UseApplicationConsole(e => e.Key == Key.F12)
                .UseILMapper()
                .UseLogin<LoginViewModel>()
                .UseDefaultShell<MainViewModel>()
                .UseNavigation(HRNavigation.Navigations)
                .UseToast()
                .UseDialogs()
                //.UseLinqRepositoryManager<HRDBManager,HRRepositoryManager>($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={dbLocation}\HRDatabase.mdf;Integrated Security=True;Connect Timeout=30",
                //                                                           "HR.Data.LinqToSQL",
                //                                                           true)
                .UseRESTRepositoryManager("http://localhost:15297/api/", OnUnauthorizedRequest, "HR.Data.REST", true)
                //.UseMongoDBRepository("mongodb://192.168.1.160:27017", "HR","HR.Data.Mongo",true)
                .Start();
        }


        public class Person : IUniqueObject<int>
        {
            public int ID { get; set; }
            public string FirstName { get; set; }
            public int GetKey
            {
                get { return ID; }
            }
        }

        public class PersonDbModel : IUniqueObject<int>
        {
            public int ID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
            public int GetKey
            {
                get { return ID; }
            }
        }

        private void OnUnauthorizedRequest()
        {
            MessageBox.Show("Unauthorized request");
        }
    }
}