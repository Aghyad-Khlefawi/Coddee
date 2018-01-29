using System;
using System.IO;
using Coddee;
using Coddee.AppBuilder;
using Coddee.Loggers;
using Coddee.Unity;
using Coddee.Windows.AppBuilder;

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
        }
    }

}
