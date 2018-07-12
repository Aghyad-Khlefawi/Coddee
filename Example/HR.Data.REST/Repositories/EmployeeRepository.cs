//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Coddee tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Coddee;
using Coddee.Data.REST;
using HR.Data.Models;
using HR.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coddee.Data;

namespace HR.Data.Rest.Repositories
{

    [Coddee.Data.RepositoryAttribute(typeof(IEmployeeRepository))]
    public class EmployeeRepository : CRUDRESTRepositoryBase<Employee, int>, IEmployeeRepository
    {
        public EmployeeRepository() :
                base("Employee")
        {
        }

        public async Task<EmployeeJob> InsertEmployeeJob(EmployeeJob item)
        {
            var res = await PostToController<EmployeeJob>(nameof(InsertEmployeeJob), item);
            EmployeeJobsChanged?.Invoke(this, new RepositoryChangeEventArgs<EmployeeJob>(OperationType.Add, item));
            return res;
        }

        public Task<IEnumerable<EmployeeJob>> GetEmployeeJobsByEmployee(int employeeId)
        {
            return GetFromController<IEnumerable<EmployeeJob>>(KeyValue(nameof(employeeId), employeeId));
        }

        public Task<IEnumerable<Employee>> GetItemsWithDetailes()
        {
            return GetFromController<IEnumerable<Employee>>();
        }

        public Task<Employee> GetItemWithDetailes(int employeeId)
        {
            return GetFromController<Employee>(KeyValue(nameof(employeeId), employeeId));
        }

        public async Task DeleteEmployeeJob(EmployeeJob employeeJob)
        {
            await PostToController(nameof(DeleteEmployeeJob), employeeJob);
            EmployeeJobsChanged?.Invoke(this, new RepositoryChangeEventArgs<EmployeeJob>(OperationType.Delete, employeeJob));
        }

        public event EventHandler<RepositoryChangeEventArgs<EmployeeJob>> EmployeeJobsChanged;
    }
}
