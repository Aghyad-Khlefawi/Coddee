// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coddee.Data;
using Coddee.Data.MongoDB;
using HR.Data.Models;
using HR.Data.Repositories;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace HR.Data.Mongo.Repositories
{
    [Repository(typeof(ICompanyRepository))]
    public class CompanyRepository : CRUDMongoRepositoryBase<Company, Guid>, ICompanyRepository
    {
        public CompanyRepository() : base(HRMongoCollections.Companies)
        {
        }
        
        protected override void ConfigureTableMappings(BsonClassMap<Company> bsonClassMap)
        {
            bsonClassMap.UnmapProperty(e => e.StateName);
        }
         
        public override Task<Company> InsertItem(Company item)
        {
            item.ID = Guid.NewGuid();
            return base.InsertItem(item);
        }

        public override async Task<Company> UpdateItem(Company item)
        {
            var update = Builders<Company>.Update
                .Set(e => e.Name, item.Name)
                .Set(e => e.StateID, item.StateID);
            await _collection.UpdateOneAsync(e => e.ID == item.ID, update);
            return item;
        }

        public async Task<IEnumerable<Company>> GetDetailedItems()
        {
            return (await _collection.Find(e => true).ToListAsync()).Select(e =>
            {
                e.StateName = _database.GetCollection<State>(HRMongoCollections.States)
                    .Find(s => s.ID == e.StateID)
                    .First()
                    .Name;
                return e;
            });
        }
    }
}