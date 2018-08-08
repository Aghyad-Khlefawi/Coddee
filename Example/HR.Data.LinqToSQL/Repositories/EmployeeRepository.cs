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
using Coddee.Data;
using HR.Data.Models;
using HR.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace HR.Data.Linq.Repositories
{

    [Coddee.Data.RepositoryAttribute(typeof(IEmployeeRepository))]
    public class EmployeeRepository : CRUDHRRepositoryBase<DB.Employee, Employee, int>, IEmployeeRepository
    {
        public override void RegisterMappings(IObjectMapper mapper)
        {
            base.RegisterMappings(mapper);
            mapper.RegisterTwoWayMap<DB.EmployeeJob, EmployeeJob>();
            mapper.RegisterMap<DB.EmployeeJobsView, EmployeeJob>();
            mapper.RegisterMap<DB.EmployeesView, Employee>();
        }

        public Task<EmployeeJob> InsertEmployeeJob(EmployeeJob item)
        {
            return Execute(db =>
            {
                var dbItem = _mapper.Map<DB.EmployeeJob>(item);
                db.EmployeeJobs.InsertOnSubmit(dbItem);
                EmployeeJobsChanged?.Invoke(this, new RepositoryChangeEventArgs<EmployeeJob>(OperationType.Add, item));
                return item;
            });
        }

        public Task<IEnumerable<EmployeeJob>> GetEmployeeJobsByEmployee(int employeeId)
        {
            return ExecuteAndMapCollection<EmployeeJob>(db => db.EmployeeJobsViews.ToList());
        }

        public Task<IEnumerable<Employee>> GetItemsWithDetailes()
        {
            return ExecuteAndMapCollection(db => db.EmployeesViews.ToList());
        }

        public Task<Employee> GetItemWithDetailes(int employeeId)
        {
            return ExecuteAndMap(db => db.EmployeesViews.First(e => e.Id == employeeId));
        }

        public Task DeleteEmployeeJob(EmployeeJob employeeJob)
        {
            return Execute(db =>
            {
                var job = db.EmployeeJobs.FirstOrDefault(e => e.BranchId == employeeJob.BranchId && e.DepartmentId == employeeJob.DepartmentId && e.JobId == employeeJob.JobId && e.EmployeeId == employeeJob.EmployeeId);
                if (job != null)
                    db.EmployeeJobs.DeleteOnSubmit(job);
                db.SubmitChanges();
                EmployeeJobsChanged?.Invoke(this, new RepositoryChangeEventArgs<EmployeeJob>(OperationType.Delete, employeeJob));
            });
        }

        public event EventHandler<RepositoryChangeEventArgs<EmployeeJob>> EmployeeJobsChanged;

        public Task<IEnumerable<Employee>> GetItemsWithDetailesByBranch(int branchId)
        {
            return ExecuteAndMapCollection(db =>
            {
                var employees = db.EmployeeJobs.Where(e => e.BranchId == branchId)
                                  .Select(e => e.EmployeeId)
                                  .Distinct()
                                  .Select(e => db.EmployeesViews.First(v => v.Id == e))
                                  .ToList();
                return employees;
            });
        }
    }
}
