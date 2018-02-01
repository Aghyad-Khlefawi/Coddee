// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coddee;
using Coddee.Data;
using Coddee.Data.MongoDB;
using HR.Data.Models;
using HR.Data.Repositories;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace HR.Data.Mongo.Repositories
{
    [Repository(typeof(IEmployeeRepository))]
    public class EmployeeRepository : MongoRepositoryBase<Employee>, IEmployeeRepository
    {
        public override void Initialize(IMongoDBManager dbManager,
                                        IRepositoryManager repositoryManager,
                                        IObjectMapper mapper,
                                        Type implementedInterface,
                                        RepositoryConfigurations config =null)
        {
            base.Initialize(dbManager, repositoryManager, mapper, implementedInterface, config);

            BsonClassMap.RegisterClassMap<Employee>(c =>
            {
                ConfigureDetaultTableMappings(c,e=>e.ID);
                c.UnmapProperty(e=>e.StateName);
                c.UnmapProperty(e=>e.CompanyName);
            });

            _companyCollection = _database.GetCollection<Company>(HRMongoCollections.Companies);
        }

        private IMongoCollection<Company> _companyCollection;

      


        public Task<Employee> this[Guid index]
        {
            get
            {
                var res = _companyCollection
                    .Find(Builders<Company>.Filter.Where(e=>e.Employees.Any(w=>w.ID == index)))
                    .Project<Company>(Builders<Company>.Projection.Include($"{nameof(Company.Employees)}.$"))
                    .ToList()
                    .First()
                    .Employees.FirstOrDefault(e => e.ID == index);
                return Task.FromResult(res);
            }
        }

        public async Task<IEnumerable<Employee>> GetItems()
        {
            return (await _companyCollection
                    .Find(e => true)
                    .Project(e => e.Employees)
                    .ToListAsync())
                .SelectMany(e => e)
                .AsEnumerable();
        }

       
        public event EventHandler<RepositoryChangeEventArgs<Employee>> ItemsChanged;

        public async Task<Employee> UpdateItem(Employee item)
        {
            var update =
                Builders<Company>.Update
                    .Set(e => e.Employees[-1].FirstName, item.FirstName)
                    .Set(e => e.Employees[-1].LastName, item.LastName);

            await _companyCollection.UpdateOneAsync(e => e.ID == item.CompanyID && e.Employees.Any(em=>em.ID == item.ID), update);
            return item;
        }

        public async Task<Employee> InsertItem(Employee item)
        {
            item.ID = Guid.NewGuid();
            await _companyCollection.UpdateOneAsync(e => e.ID == item.CompanyID,
                                                    Builders<Company>.Update.Push(e => e.Employees, item));
            return item;
        }

        public async Task DeleteItemByKey(Guid ID)
        {
            var item = await this[ID];
            await _companyCollection.FindOneAndUpdateAsync(e => e.ID == item.CompanyID,
                                                    Builders<Company>.Update.Pull(e => e.Employees, item));
        }

        public Task DeleteItem(Employee item)
        {
            return DeleteItemByKey(item.ID);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByCompany(Guid companyID)
        {
            var companyRepo = _repositoryManager.GetRepository<ICompanyRepository>();
            var stateRepo = _repositoryManager.GetRepository<IStateRepository>();

            var employees = await _companyCollection
                .Find(e => e.ID == companyID)
                .Project(e => e.Employees)
                .ToListAsync();

            var res = new List<Employee>();
            foreach (var employeeList in employees)
            {
                if (employeeList != null)
                {
                    res.AddRange(employeeList.Select(e =>
                                     {
                                         var company = companyRepo[e.CompanyID].Result;
                                         e.CompanyName = company.Name;
                                         e.StateName = stateRepo[company.StateID.GetValueOrDefault()].Result.Name;
                                         return e;
                                     })
                                     .ToList());
                }
            }
            return res;
        }
    }
}