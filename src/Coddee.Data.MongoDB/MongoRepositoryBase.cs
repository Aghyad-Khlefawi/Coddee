// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Coddee.Data.MongoDB
{
    /// <summary>
    /// Base implementation for a MongoDB repository
    /// </summary>
    public abstract class MongoRepositoryBase : RepositoryBase, IMongoRepository
    {
        protected IMongoDBManager _dbManager;

        /// <summary>
        /// Do any required initialization
        /// </summary>
        public void Initialize(IMongoDBManager dbManager,
                               IRepositoryManager repositoryManager,
                               IObjectMapper mapper,
                               Type implementedInterface)
        {
            _dbManager = dbManager;
            Initialize(repositoryManager, mapper, implementedInterface);
        }
    }

    /// <summary>
    /// Base implementation for a MongoDB repository
    /// <remarks>
    /// Implements the ReadOnly functionality
    /// </remarks>
    /// </summary>
    public abstract class ReadOnlyMongoRepositoryBase<TModel, TKey> : MongoRepositoryBase,
        IReadOnlyRepository<TModel, TKey>
    {
        protected readonly Expression<Func<TModel, TKey>> _idProperty;
        protected readonly string _collectionName;
        protected IMongoCollection<TModel> _collection;

        protected ReadOnlyMongoRepositoryBase(string collectionName)
        {
            _collectionName = collectionName;
        }

        protected ReadOnlyMongoRepositoryBase(string collectionName, Expression<Func<TModel, TKey>> idProperty)
            : this(collectionName)
        {
            _idProperty = idProperty;
        }

        public Task<TModel> this[TKey index] => _collection.Find(new BsonDocument("_id", BsonValue.Create(index)))
            .FirstAsync();

        public async Task<IEnumerable<TModel>> GetItems()
        {
            return (await _collection.Find(e => true).ToListAsync()).AsEnumerable();
        }

        protected virtual void RegisterTableMappings()
        {
            BsonClassMap.RegisterClassMap<TModel>(c =>
            {
                ConfigureDetaultTableMappings(c, _idProperty);
                ConfigureTableMappings(c);
            });
        }

        protected virtual string GetDefaultIdColumnName()
        {
            return "ID";
        }

        protected virtual void ConfigureDetaultTableMappings<TType>(
            BsonClassMap<TType> c,
            Expression<Func<TType, TKey>> idMap)
        {
            c.AutoMap();
            if (idMap != null)
                c.MapIdMember(idMap);
            else
                c.MapIdProperty(GetDefaultIdColumnName());
            c.SetIgnoreExtraElements(true);
        }

        protected virtual void ConfigureTableMappings<TType>(BsonClassMap<TType> bsonClassMap)
        {
        }
    }


    /// <summary>
    /// Base implementation for a MongoDB repository
    /// <remarks>
    /// Implements the CRUD functionality
    /// </remarks>
    /// </summary>
    public abstract class CRUDMongoRepositoryBase<TModel, TKey> : ReadOnlyMongoRepositoryBase<TModel, TKey>,
        ICRUDRepository<TModel, TKey> where TModel : IUniqueObject<TKey>
    {
        protected CRUDMongoRepositoryBase(string collectionName) : base(collectionName)
        {
        }

        protected CRUDMongoRepositoryBase(string collectionName,
                                          Expression<Func<TModel, TKey>> idProperty) : base(collectionName, idProperty)
        {
        }

        public async Task<TModel> UpdateItem(TModel item)
        {
            await _collection.ReplaceOneAsync(new BsonDocument("_id", BsonValue.Create(item.GetKey)), item);
            return item;
        }

        public async Task<TModel> InsertItem(TModel item)
        {
            await _collection.InsertOneAsync(item);
            return item;
        }

        public Task DeleteItem(TKey ID)
        {
            return _collection.DeleteOneAsync(new BsonDocument("_id", BsonValue.Create(ID)));
        }

        public Task DeleteItem(TModel item)
        {
            return DeleteItem(item.GetKey);
        }
    }
}