using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Coddee.Collections;
using Coddee.Xamarin.Forms;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Clients.Components
{
    public class EmployeesViewModel:ViewModelBase<EmployeesPage>
    {
        private Branch _branch;

        private AsyncObservableCollection<Employee> _employees;
        public AsyncObservableCollection<Employee> Employees
        {
            get { return _employees; }
            set { SetProperty(ref _employees, value); }
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            Employees = AsyncObservableCollection<Employee>.Create();
        }

        public async Task SetBranch(Branch branch)
        {
            _branch = branch;
            await ToggleBusyAsync(Employees.ClearAndFillAsync(GetRepository<IEmployeeRepository>().GetItemsWithDetailesByBranch(branch.Id)));
        }
    }
}
