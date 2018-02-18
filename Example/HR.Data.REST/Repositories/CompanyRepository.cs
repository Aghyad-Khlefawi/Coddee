// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coddee.Data;
using Coddee.Data.REST;
using HR.Data.Models;
using HR.Data.Repositories;
using Newtonsoft.Json;

namespace HR.Data.REST.Repositories
{
    [Repository(typeof(ICompanyRepository))]
    public class CompanyRepository : CRUDRESTRepositoryBase<Models.Company, Guid>, ICompanyRepository
    {
        public CompanyRepository()
            : base("Company")
        {

        }
        public Task<IEnumerable<Company>> GetDetailedItems()
        {
            return GetFromController<IEnumerable<Company>>(nameof(GetDetailedItems));
        }
    }
}
