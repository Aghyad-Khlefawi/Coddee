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
        protected IMongoDatabase _database;

        /// <summary>
        /// Do any required initialization
        /// </summary>
        public virtual void Initialize(IMongoDBManager dbManager,
                                       IRepositoryManager repositoryManager,
                                       IObjectMapper mapper,
                                       Type implementedInterface,
                                       RepositoryConfigurations config = null)
        {
            _dbManager = dbManager;
            _database = _dbManager.GetDatabase();
            Initialize(repositoryManager, mapper, implementedInterface,config);
        }

        protected virtual void ConfigureDetaultTableMappings<TType, TKey>(
            BsonClassMap<TType> c,
            Expression<Func<TType, TKey>> idMap)
        {
            c.AutoMap();
            if (idMap != null)
                c.MapIdMember(idMap);
            c.SetIgnoreExtraElements(true);
        }

        protected virtual void ConfigureDetaultTableMappings<TType>(
            BsonClassMap<TType> c,
            string idColumn)
        {
            c.AutoMap();
            c.MapIdProperty(idColumn);
            c.SetIgnoreExtraElements(true);
        }
    }

    /// <summary>
    /// Base implementation for a MongoDB repository
    /// Register class mapping for a collection 
    /// </summary>
    public abstract class MongoRepositoryBase<TModel, TKey> : MongoRepositoryBase
    {
        protected IMongoCollection<TModel> _collection;
        protected readonly Expression<Func<TModel, TKey>> _idProperty;
        protected readonly string _collectionName;
        public event EventHandler<RepositoryChangeEventArgs<TModel>> ItemsChanged;

        private readonly string _identifier;

        protected MongoRepositoryBase(string collectionName)
        {
            _collectionName = collectionName;
            _identifier = typeof(TModel).Name;
        }

        protected MongoRepositoryBase(string collectionName, Expression<Func<TModel, TKey>> idProperty)
            : this(collectionName)
        {
            _idProperty = idProperty;
        }


        /// <summary>
        /// Do any required initialization
        /// </summary>
        public override void Initialize(IMongoDBManager dbManager,
                                        IRepositoryManager repositoryManager,
                                        IObjectMapper mapper,
                                        Type implementedInterface,
                                        RepositoryConfigurations config = null)
        {
            base.Initialize(dbManager, repositoryManager, mapper, implementedInterface,config);
            RegisterTableMappings();
            _collection = _database.GetCollection<TModel>(_collectionName);
        }


        protected virtual void RegisterTableMappings()
        {
            BsonClassMap.RegisterClassMap<TModel>(c =>
            {
                if (_idProperty != null)
                    ConfigureDetaultTableMappings(c, _idProperty);
                else
                    ConfigureDetaultTableMappings(c, GetDefaultIdColumnName());

                ConfigureTableMappings(c);
            });
        }

        protected virtual string GetDefaultIdColumnName()
        {
            return "ID";
        }


        protected virtual void ConfigureTableMappings(BsonClassMap<TModel> bsonClassMap)
        {
        }
        public override void SyncServiceSyncReceived(string identifier, RepositorySyncEventArgs args)
        {
            base.SyncServiceSyncReceived(identifier, args);
            if (identifier == _identifier)
                RaiseItemsChanged(this,
                                  new RepositoryChangeEventArgs<TModel>(args.OperationType, (TModel)args.Item));
        }

        protected void RaiseItemsChanged(object sender, RepositoryChangeEventArgs<TModel> args)
        {
            ItemsChanged?.Invoke(this,
                                 new RepositoryChangeEventArgs<TModel>(args.OperationType, args.Item));
        }

        public override void SetSyncService(IRepositorySyncService syncService)
        {
            base.SetSyncService(syncService);
            ItemsChanged += OnItemsChanged;
        }
        private void OnItemsChanged(object sender, RepositoryChangeEventArgs<TModel> e)
        {
            _syncService?.SyncItem(_identifier, new RepositorySyncEventArgs { Item = e.Item, OperationType = e.OperationType });
        }
    }

    /// <summary>
    /// Base implementation for a MongoDB repository
    /// <remarks>
    /// Implements the ReadOnly functionality
    /// </remarks>
    /// </summary>
    public abstract class ReadOnlyMongoRepositoryBase<TModel, TKey> : MongoRepositoryBase<TModel, TKey>,
        IReadOnlyRepository<TModel, TKey> where TModel : IUniqueObject<TKey>
    {
        protected ReadOnlyMongoRepositoryBase(string collectionName)
            : base(collectionName)
        {
        }

        protected ReadOnlyMongoRepositoryBase(string collectionName, Expression<Func<TModel, TKey>> idProperty)
            : base(collectionName, idProperty)
        {
        }


        public virtual Task<TModel> this[TKey index] => _collection
            .Find(new BsonDocument("_id", BsonValue.Create(index)))
            .FirstAsync();

        public virtual async Task<IEnumerable<TModel>> GetItems()
        {
            return (await _collection.Find(e => true).ToListAsync()).AsEnumerable();
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
        

        public virtual async Task<TModel> UpdateItem(TModel item)
        {
            await _collection.ReplaceOneAsync(new BsonDocument("_id", BsonValue.Create(item.GetKey)), item);
            RaiseItemsChanged(this,new RepositoryChangeEventArgs<TModel>(OperationType.Edit, item));
            return item;
        }

        public virtual async Task<TModel> InsertItem(TModel item)
        {
            await _collection.InsertOneAsync(item);
            RaiseItemsChanged(this,new RepositoryChangeEventArgs<TModel>(OperationType.Add, item));
            return item;
        }

        public virtual async Task DeleteItem(TKey ID)
        {
            var item = await this[ID];
            await _collection.DeleteOneAsync(new BsonDocument("_id", BsonValue.Create(ID)));
            RaiseItemsChanged(this,new RepositoryChangeEventArgs<TModel>(OperationType.Edit, item));
        }

        public  virtual async Task DeleteItem(TModel item)
        {
            await DeleteItem(item.GetKey);
            RaiseItemsChanged(this,new RepositoryChangeEventArgs<TModel>(OperationType.Edit, item));
        }
    }
}