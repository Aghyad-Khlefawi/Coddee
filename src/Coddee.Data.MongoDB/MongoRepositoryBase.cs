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
using Newtonsoft.Json.Linq;

namespace Coddee.Data.MongoDB
{
    /// <summary>
    /// Base implementation for a MongoDB repository
    /// </summary>
    public abstract class MongoRepositoryBase<TModel> : RepositoryBase<TModel>, IMongoRepository
    {
        /// <summary>
        /// The db manager for creating sessions.
        /// </summary>
        protected IMongoDBManager _dbManager;


        /// <summary>
        /// A reference to the database.
        /// </summary>
        protected IMongoDatabase _database;

        /// <inheritdoc />
        public override int RepositoryType { get; } = (int)RepositoryTypes.Mongo;


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
            Initialize(repositoryManager, mapper, implementedInterface, config);
        }


        /// <summary>
        /// Configure the default mapping for the model.
        /// </summary>
        protected virtual void ConfigureDetaultTableMappings<TType, TKey>(
            BsonClassMap<TType> c,
            Expression<Func<TType, TKey>> idMap)
        {
            c.AutoMap();
            if (idMap != null)
                c.MapIdMember(idMap);
            c.SetIgnoreExtraElements(true);
        }

        /// <summary>
        /// Configure the default mapping for the model.
        /// </summary>
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
    public abstract class MongoRepositoryBase<TModel, TKey> : MongoRepositoryBase<TModel>, IRepository<TModel, TKey> where TModel : IUniqueObject<TKey>
    {
        /// <summary>
        /// A reference to the database collection.
        /// </summary>
        protected IMongoCollection<TModel> _collection;

        /// <summary>
        /// A function that returns the key for a model object.
        /// </summary>
        protected readonly Expression<Func<TModel, TKey>> _idProperty;

        /// <summary>
        /// The name of the database collection.
        /// </summary>
        protected readonly string _collectionName;

        /// <inheritdoc />
        public event EventHandler<RepositoryChangeEventArgs<TModel>> ItemsChanged;

        private readonly string _identifier;

        /// <inheritdoc />
        protected MongoRepositoryBase(string collectionName)
        {
            _collectionName = collectionName;
            _identifier = typeof(TModel).Name;
        }

        /// <inheritdoc />
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
            base.Initialize(dbManager, repositoryManager, mapper, implementedInterface, config);
            RegisterTableMappings();
            _collection = _database.GetCollection<TModel>(_collectionName);
        }

        /// <summary>
        /// Register the mapping information for the table.
        /// </summary>
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

        /// <summary>
        /// Returns the default name used for the Id column.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetDefaultIdColumnName()
        {
            return "ID";
        }

        /// <summary>
        /// Configure additional table mapping
        /// </summary>
        protected virtual void ConfigureTableMappings(BsonClassMap<TModel> bsonClassMap)
        {
        }

        /// <inheritdoc />
        public override void SyncServiceSyncReceived(string identifier, RepositorySyncEventArgs args)
        {
            base.SyncServiceSyncReceived(identifier, args);
            if (identifier == _identifier)
                RaiseItemsChanged(this,
                                  new RepositoryChangeEventArgs<TModel>(args.OperationType, ((JObject)args.Item).ToObject<TModel>(), true));
        }

        /// <summary>
        /// Called when the repository content is changed
        /// </summary>
        protected virtual void RaiseItemsChanged(object sender, RepositoryChangeEventArgs<TModel> args)
        {
            ItemsChanged?.Invoke(this, args);
        }

        /// <inheritdoc />
        public override void SetSyncService(IRepositorySyncService syncService, bool sendSyncRequests = true)
        {
            base.SetSyncService(syncService, sendSyncRequests);
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
        /// <inheritdoc />
        protected ReadOnlyMongoRepositoryBase(string collectionName)
            : base(collectionName)
        {
        }

        /// <inheritdoc />
        protected ReadOnlyMongoRepositoryBase(string collectionName, Expression<Func<TModel, TKey>> idProperty)
            : base(collectionName, idProperty)
        {
        }


        /// <inheritdoc />
        public virtual Task<TModel> this[TKey index] => _collection
            .Find(new BsonDocument("_id", BsonValue.Create(index)))
            .FirstAsync();

        /// <inheritdoc />
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
        /// <inheritdoc />
        protected CRUDMongoRepositoryBase(string collectionName) : base(collectionName)
        {
        }

        /// <inheritdoc />
        protected CRUDMongoRepositoryBase(string collectionName,
                                          Expression<Func<TModel, TKey>> idProperty) : base(collectionName, idProperty)
        {
        }


        /// <inheritdoc />
        public virtual async Task<TModel> UpdateItem(TModel item)
        {
            await _collection.ReplaceOneAsync(new BsonDocument("_id", BsonValue.Create(item.GetKey)), item);
            RaiseItemsChanged(this, new RepositoryChangeEventArgs<TModel>(OperationType.Edit, item, false));
            return item;
        }

        /// <inheritdoc />
        public virtual async Task<TModel> InsertItem(TModel item)
        {
            await _collection.InsertOneAsync(item);
            RaiseItemsChanged(this, new RepositoryChangeEventArgs<TModel>(OperationType.Add, item, false));
            return item;
        }

        /// <inheritdoc />
        public virtual async Task DeleteItemByKey(TKey ID)
        {
            var item = await this[ID];
            await _collection.DeleteOneAsync(new BsonDocument("_id", BsonValue.Create(ID)));
            RaiseItemsChanged(this, new RepositoryChangeEventArgs<TModel>(OperationType.Edit, item, false));
        }

        /// <inheritdoc />
        public virtual async Task DeleteItem(TModel item)
        {
            await DeleteItemByKey(item.GetKey);
            RaiseItemsChanged(this, new RepositoryChangeEventArgs<TModel>(OperationType.Edit, item, false));
        }
    }
}