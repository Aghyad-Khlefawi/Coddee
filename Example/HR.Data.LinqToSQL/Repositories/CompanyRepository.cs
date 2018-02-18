// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coddee;
using Coddee.Data;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Data.LinqToSQL.Repositories
{
    [Repository(typeof(ICompanyRepository))]
    public class CompanyRepository : CRUDHRRepositoryBase<DB.Company, Company, Guid>, ICompanyRepository
    {
        public CompanyRepository()
        {
            
        }
        public override void RegisterMappings(IObjectMapper mapper)
        {
            base.RegisterMappings(mapper);
            mapper.RegisterAutoMap<DB.CompaniesView, Company>();
        }

        public override async Task<Company> InsertItem(Company item)
        {
            var res = await base.InsertItem(item);
            return await Execute(db => { return _mapper.Map<Company>(db.CompaniesViews.First(e => e.ID == res.ID)); });
        }

        public Task<IEnumerable<Company>> GetDetailedItems()
        {
            return Execute(db => _mapper.MapCollection<Company>(db.CompaniesViews.ToList()));
        }

        public override async Task<Company> UpdateItem(Company item)
        {
            var res = await base.UpdateItem(item);
            return await Execute(db => { return _mapper.Map<Company>(db.CompaniesViews.First(e => e.ID == res.ID)); });
        }
    }
}