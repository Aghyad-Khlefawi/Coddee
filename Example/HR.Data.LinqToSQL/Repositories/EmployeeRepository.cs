// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coddee;
using Coddee.Data;
using Coddee.Data.LinqToSQL;
using HR.Data.LinqToSQL.DB;
using HR.Data.Repositories;
using Employee = HR.Data.Models.Employee;

namespace HR.Data.LinqToSQL.Repositories
{
    [Repository(typeof(IEmployeeRepository))]
    public class EmployeeRepository:CRUDHRRepositoryBase<DB.Employee,Models.Employee,Guid>, IEmployeeRepository
    {
        public override void Initialize(LinqDBManager<HRDataClassesDataContext> dbManager,
                                        IRepositoryManager repositoryManager,
                                        IObjectMapper mapper,
                                        Type implementedInterface,
                                        RepositoryConfigurations config = null)
        {
            base.Initialize(dbManager, repositoryManager, mapper, implementedInterface, config);
        }

        public override void RegisterMappings(IObjectMapper mapper)
        {
            base.RegisterMappings(mapper);
            mapper.RegisterMap<DB.EmployeesView,Employee>();
        }

        public override async Task<Employee> InsertItem(Employee item)
        {
            var res = await base.InsertItem(item);
            return await Execute(db =>
            {
                return _mapper.Map<Employee>(db.EmployeesViews.First(e => e.ID == res.ID));
            });
        }

        public override async Task<Employee> UpdateItem(Employee item)
        {
            var res = await base.UpdateItem(item);
            return await Execute(db =>
            {
                return _mapper.Map<Employee>(db.EmployeesViews.First(e => e.ID == res.ID));
            });
        }

        public Task<IEnumerable<Employee>> GetEmployeesByCompany(Guid companyID)
        {
            return Execute(db => _mapper.MapCollection<Employee>(db.EmployeesViews.Where(e => e.CompanyID == companyID).ToList()));
        }
    }
}