using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Coddee.WPF.Modules
{
    public interface IDebugTool
    {

        Task Initialize();
        
        /// <summary>
        /// Set the condition on which the tool visibility will be toggled
        /// </summary>
        /// <param name="toggleCondition"></param>
        void SetToggleCondition(Func<KeyEventArgs, bool> toggleCondition);

    }
}
