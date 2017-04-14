// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coddee.Data;
using Coddee.Data.Rest;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Data.REST.Repositories
{
    [Repository(typeof(IEmployeeRepository))]
    public class EmployeeRepository: CRUDRESTRepositoryBase<Models.Employee,Guid>, IEmployeeRepository
    {
        public override string ControllerName => "Employee";

        public Task<IEnumerable<Employee>> GetEmployeesByCompany(Guid companyID)
        {
            return GetFromController<IEnumerable<Employee>>(nameof(GetEmployeesByCompany),
                                                            new KeyValuePair<string, string>(nameof(companyID),
                                                                                             companyID.ToString()));
        }
    }
}