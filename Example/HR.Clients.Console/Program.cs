﻿using System;
using System.IO;
using System.Linq;
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
        public void Start()
        {
            var temp = new AsyncObservableDictionary<Guid, Employee>();
            var id = Guid.NewGuid();
            temp.Add(new Employee { ID = id });
            temp.Update(OperationType.Edit, new Employee { ID = id, FirstName = "Test" });
        }
    }
}
