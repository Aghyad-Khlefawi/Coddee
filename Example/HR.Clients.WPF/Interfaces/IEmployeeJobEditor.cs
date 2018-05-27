using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.Mvvm;
using HR.Data.Models;

namespace HR.Clients.WPF.Interfaces
{
    public interface IEmployeeJobEditor : IEditorViewModel<EmployeeJob>
    {
        void AddFromEmployee(int employeeId);
    }
}
