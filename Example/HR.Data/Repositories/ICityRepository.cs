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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace HR.Data.Repositories
{
    
    public interface ICityRepository : ICRUDRepository<City, int>
    {
        Task<IEnumerable<City>> GetItemsWithDetails();
    }
}
