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
                   .UseMain<HRApplication>();
                //.UseLinqRepositoryManager<HRDBManager>(connection, "HR.Data.LinqToSQL");
            });
        }
    }


    public class HRApplication : IEntryPointClass
    {
        private readonly IObjectMapper _mapper;

        public HRApplication(IObjectMapper mapper)
        {
            _mapper = mapper;
        }


        public void Start(IContainer container)
        {

            var c2 = new Class2 { Age = 5 };
            var temp2 = new Class1();
            if (c2.Age != null)
            {
                object temp = c2.Age;
                temp2.Age = (int)temp;
            }
            _mapper.RegisterAutoMap<Class2, Class1>((source, target) =>
            {
                target.Age = 10;
            });
            var res = _mapper.MapCollection<Class1>(new List<Class2>{ c2 });
            //var res3 = _mapper.Map<Class1>( c2 );
        }
    }

    public class Class1
    {
        public int Age { get; set; }
    }
    public class Class2
    {
        public int? Age { get; set; }
    }
}
