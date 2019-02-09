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
    [Repository(typeof(ICompanyRepository))]
    public class CompanyRepository : CRUDFileRepositoryBase<Company, int>, ICompanyRepository
    {
        public CompanyRepository() : base("Company")
        {

        }

        public Task<IEnumerable<Company>> GetItemsWithDetails(DateTime test)
        {
            return GetItems();
        }
    }
}
