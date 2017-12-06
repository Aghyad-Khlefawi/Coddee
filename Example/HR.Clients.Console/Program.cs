using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Coddee;
using Coddee.AppBuilder;
using Coddee.Collections;
using Coddee.Data;
using Coddee.Loggers;
using Coddee.Unity;
using Coddee.Windows.AppBuilder;
using HR.Data.LinqToSQL;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbLocation = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB"));
            var connection = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={dbLocation}\HRDatabase.mdf;Integrated Security=True;Connect Timeout=30";

            new ConsoleApplication("HR Application", new CoddeeUnityContainer()).Run(app =>
             {
                 app.UseLogger(LoggerTypes.DebugOutput | LoggerTypes.ApplicationConsole, LogRecordTypes.Information)
                     .UseILMapper()
                     .UseMain<HRApplication>()
                     .UseLinqRepositoryManager<HRDBManager>(connection, "HR.Data.LinqToSQL");
             });
        }
    }


    public class HRApplication : IEntryPointClass
    {
        private readonly IRepositoryManager _repositoryManager;

        public HRApplication(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public int Age1 { get; set; }
        public int? Age2 { get; set; }
        public void Start(IContainer container)
        {
            Age1 = 5;
            GetType().GetProperty(nameof(Age2)).SetMethod.Invoke(this, new object[]{ GetType().GetProperty(nameof(Age1)).GetMethod.Invoke(this,null) });
        }
    }
   
}
