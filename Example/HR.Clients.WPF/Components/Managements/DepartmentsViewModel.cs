using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.WPF;
using HR.Clients.WPF.Interfaces;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.WPF.Components.Managements
{
    public class DepartmentsViewModel : ManagementViewModelBase<DepartmentsView, IDepartmentEditor, IDepartmentRepository, Department, int>
    {
        protected override bool Filter(Department item, string term)
        {
            return item.Title.ToLower().Contains(term.ToLower());
        }

        public override string Title { get; } = "Departments";
    }
}