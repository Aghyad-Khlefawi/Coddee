using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.WPF.Modules;
using Microsoft.Practices.Unity;

namespace Coddee.WPF.DebugTool
{
    [Module(BuiltInModules.DebugTool)]
    public class DebugToolModule:IModule
    {
        public void Initialize(IUnityContainer container)
        {
            container.RegisterInstance<IDebugTool,DebugToolViewModel>();
        }
    }
}