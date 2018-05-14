using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.Mvvm;

namespace HR.Clients.WPF
{
    public interface IManagementViewModel:IViewModel
    {
        string Title { get; }
    }
}
