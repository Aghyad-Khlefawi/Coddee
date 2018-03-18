// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coddee.Attributes;
using Coddee.Data;
using HR.Data.Models;

namespace HR.Data.Repositories
{
        [Authorize]
    public interface ICompanyRepository : ICRUDRepository<Company, Guid>
    {
        //[Authorize]
        //new Task<IEnumerable<Company>> GetItems();

        Task<IEnumerable<Company>> GetDetailedItems();
    }
}
