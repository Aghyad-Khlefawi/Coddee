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
            new ConsoleApplication("HR Application", new CoddeeUnityContainer()).Run(app =>
             {
                 app.UseLogger(LoggerTypes.DebugOutput|LoggerTypes.ApplicationConsole, LogRecordTypes.Information)
                     .UseILMapper()
                     .UseMain<HRApplication>();
             });
        }
    }

   
    public class HRApplication:IEntryPointClass
    {
      public void Start()
        {
            System.Console.WriteLine("Hello console");
            System.Console.Read();
        }
    }
}
