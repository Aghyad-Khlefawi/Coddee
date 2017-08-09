using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.AppBuilder;

namespace Coddee.AppBuilder
{
    public class ConsoleApplicationBuilder: WindowsApplicationBuilder,IConsoleApplicationBuilder
    {
        public ConsoleApplicationBuilder(IApplication app, IContainer container) : base(app, container)
        {

        }

    }
}
