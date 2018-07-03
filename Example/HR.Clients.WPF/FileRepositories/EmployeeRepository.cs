//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Coddee tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Coddee.Data;
using HR.Data.Models;
using HR.Data.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HR.Data.Rest.Repositories
{

    [Coddee.Data.RepositoryAttribute(typeof(IEmployeeRepository))]
    public class EmployeeRepository : CRUDFileRepositoryBase<Employee, int>, IEmployeeRepository
    {
        public EmployeeRepository() :
                base("Employee")
        {
        }

        private RepositoryDataFile<int, List<EmployeeJob>> _employeeJobs;

        protected override void CreateDataFiles()
        {
            base.CreateDataFiles();
            _employeeJobs = CreateDataFile<int, List<EmployeeJob>>("EmployeeJobs");
        }

        public async Task<EmployeeJob> InsertEmployeeJob(EmployeeJob item)
        {
            var employeeJobsCollection = await _employeeJobs.GetCollection();
            List<EmployeeJob> employeeJobs;
            if (!employeeJobsCollection.TryGetValue(item.EmployeeId, out employeeJobs))
            {
                employeeJobs = new List<EmployeeJob> { item };
                employeeJobsCollection.TryAdd(item.EmployeeId, employeeJobs);
            }
            else
            {
                employeeJobs.Add(item);
            }

            await _employeeJobs.UpdateFile();
            return item;
        }

        public async Task<IEnumerable<EmployeeJob>> GetEmployeeJobsByEmployee(int employeeId)
        {
            var employeeJobsCollection = await _employeeJobs.GetCollection();
            List<EmployeeJob> result = null;
            employeeJobsCollection.TryGetValue(employeeId, out result);
            return result.AsEnumerable();
        }

        public Task<IEnumerable<Employee>> GetItemsWithDetailes()
        {
            return GetItems();
        }

        public Task<Employee> GetItemWithDetailes(int employeeId)
        {
            return this[employeeId];
        }
    }
}