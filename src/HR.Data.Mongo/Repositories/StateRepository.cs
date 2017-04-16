// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using Coddee.Data;
using Coddee.Data.MongoDB;
using HR.Data.Models;
using HR.Data.Repositories;

namespace HR.Data.Mongo.Repositories
{
    [Repository(typeof(IStateRepository))]
    public class StateRepository:ReadOnlyMongoRepositoryBase<State,int>, IStateRepository
    {
        public StateRepository() : base(HRMongoCollections.States)
        {
        }
        
    }
}