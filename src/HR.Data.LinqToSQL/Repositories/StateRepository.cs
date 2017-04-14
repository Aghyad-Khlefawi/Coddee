// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.Data;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Data.LinqToSQL.Repositories
{
    [Repository(typeof(IStateRepository))]
    public class StateRepository : ReadOnlyHRRepositoryBase<DB.State, State, int>, IStateRepository
    {
        
    }
}