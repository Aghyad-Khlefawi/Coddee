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

namespace HR.Data.Rest.Repositories
{
    
    [Coddee.Data.RepositoryAttribute(typeof(ICityRepository))]
    public class CityRepository : CRUDFileRepositoryBase<City, int>, ICityRepository
    {
        public CityRepository() : 
                base("City")
        {
        }

        public Task<IEnumerable<City>> GetItemsWithDetails()
        {
            return GetItems();
        }

        public async Task<IEnumerable<City>> GetItemsByCountry(int countryId)
        {
            return (await GetRepositoryItems()).Values.Where(e=>e.CountryId == countryId);
        }
    }
}
