using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coddee.Data;
using HR.Data.Models;
using HR.Data.Repositories;
using Coddee.Data;

namespace HR.Data.FileRepositories
{
    [Repository(typeof(IBranchRepository))]
    public class BranchRepository : CRUDFileRepositoryBase<Branch, int>, IBranchRepository
    {
        public BranchRepository() : base("Branch")
        {

        }

        public Task<IEnumerable<Branch>> GetItemsWithDetails()
        {
            return GetItems();
        }

        public async Task<IEnumerable<Branch>> GetItemsWithDetailsByCompany(int companyId, DateTime now)
        {
            return (await GetRepositoryItems()).Values.Where(e => e.CompanyId == companyId);
        }
    }
}
