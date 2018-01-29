// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Coddee.Data.LinqToSQL
{
    /// <summary>
    /// Base implementation for a LinqToSQL repository
    /// </summary>
    public class LinqRepositoryBase<TDataContext> : RepositoryBase, ILinqRepository
        where TDataContext : DataContext
    {
        public override int RepositoryType { get; } = (int)RepositoryTypes.Linq;

        protected LinqDBManager<TDataContext> _dbManager;
        /// <summary>
        /// Initialize the repository
        /// </summary>
        public virtual void Initialize(
            ILinqDBManager dbManager,
            IRepositoryManager repositoryManager,
            IObjectMapper mapper,
            Type implementedInterface,
            RepositoryConfigurations config = null)
        {
            _dbManager = (LinqDBManager<TDataContext>)dbManager;
            Initialize(repositoryManager, mapper, implementedInterface, config);
        }


        /// <summary>
        /// Execute an action on the database context without returning a value
        /// </summary>
        protected Task Execute(Action<TDataContext> action)
        {
            return Task.Run(() =>
            {
                using (var context = _dbManager.CreateContext())
                {
                    action(context);
                    context.SubmitChanges();
                }
            });
        }

        protected DbTransaction CreateTransaction(TDataContext context)
        {
            if (context.Connection.State != ConnectionState.Open)
                context.Connection.Open();
            return context.Transaction = context.Connection.BeginTransaction();
        }
        /// <summary>
        /// Execute a function on the database context and then return a value
        /// </summary>
        protected Task<TResult> Execute<TResult>(Func<TDataContext, TResult> action)
        {
            return Task.Run(() =>
            {
                using (var context = _dbManager.CreateContext())
                {
                    var res = action(context);
                    context.SubmitChanges();
                    return res;
                }
            });
        }

        /// <summary>
        /// Execute an action on the database context without returning a value
        /// The action will be wrapped with a transaction 
        /// The caller is reasonable for calling transaction.Commit
        /// </summary>
        protected async Task TransactionalExecute(Action<TDataContext, DbTransaction> action)
        {
            using (var context = _dbManager.CreateContext())
            {
                CreateTransaction(context);
                await TransactionalExecute(context, action);
            }
        }

        /// <summary>
        /// Execute an action on the database context without returning a value
        /// The action will be wrapped with a transaction 
        /// The caller is reasonable for calling transaction.Commit
        /// </summary>
        protected Task TransactionalExecute(TDataContext context, Action<TDataContext, DbTransaction> action)
        {
            return Task.Run(() =>
            {
                var transaction = context.Transaction;
                try
                {
                    action(context, transaction);
                    context.SubmitChanges();
                }
                catch
                {
                    transaction?.Rollback();
                    throw;
                }
            });
        }

        /// <summary>
        /// Execute a function on the database context and then return a value
        /// The action will be wrapped with a transaction 
        /// The caller is reasonable for calling transaction.Commit
        /// </summary>
        /// <param name="action"></param>
        protected async Task<TResult> TransactionalExecute<TResult>(Func<TDataContext, DbTransaction, TResult> action)
        {
            using (var context = _dbManager.CreateContext())
            {
                CreateTransaction(context);
                return await TransactionalExecute(context, action);
            }
        }

        protected Task<TResult> TransactionalExecute<TResult>(TDataContext context, Func<TDataContext, DbTransaction, TResult> action)
        {
            return Task.Run(() =>
            {
                var transaction = context.Transaction;
                try
                {
                    var res = action(context, transaction);
                    context.SubmitChanges();
                    return res;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }

            });
        }


        /// <summary>
        /// Returns the UTC time form the connected SQL Server
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        protected DateTime GetServerUTCDate(DataContext db)
        {
            return db.ExecuteQuery<DateTime>("Select SYSUTCDATETIME()").First();
        }
    }


    /// <summary>
    /// Base implementation for a LinqToSQL repository that interact with a specific SQL table
    /// </summary>
    /// <typeparam name="TDataContext">The linqToSql DataContext</typeparam>
    /// <typeparam name="TTable">The table type</typeparam>
    public class LinqRepositoryBase<TDataContext, TTable> :
        LinqRepositoryBase<TDataContext> where TDataContext : DataContext where TTable : class, new()
    {
        /// <summary>
        /// Execute an action on the targeted SQL table without returning a value
        /// </summary>
        protected Task Execute(Action<TDataContext, Table<TTable>> action)
        {
            return Task.Run(() =>
            {
                var context = _dbManager.CreateContext();
                var table = context.GetTable<TTable>();
                using (context)
                {
                    action(context, table);
                    context.SubmitChanges();
                }
            });
        }


        /// <summary>
        /// Execute a function on the targeted SQL table and then returning a value
        /// </summary>
        protected Task<TResult> Execute<TResult>(Func<TDataContext, Table<TTable>, TResult> action)
        {
            var context = _dbManager.CreateContext();
            var table = context.GetTable<TTable>();
            return Task.Run(() =>
            {
                using (context)
                {
                    var res = action(context, table);
                    context.SubmitChanges();
                    return res;
                }
            });
        }

        /// <summary>
        /// Execute a function on the targeted SQL table and then returning a value
        /// The action will be wrapped with a transaction 
        /// The caller is reasonable for calling transaction.Commit
        /// </summary>
        /// <param name="action"></param>
        protected async Task<TResult> TransactionalExecute<TResult>(
            Func<TDataContext, Table<TTable>, DbTransaction, TResult> action)
        {
            using (var context = _dbManager.CreateContext())
            {
                var transaction = CreateTransaction(context);
                return await TransactionalExecute(context, transaction, action);
            }
        }

        /// <summary>
        /// Execute an action on the database context without returning a value
        /// The action will be wrapped with a transaction 
        /// The caller is reasonable for calling transaction.Commit
        /// </summary>
        protected Task<TResult> TransactionalExecute<TResult>(TDataContext context, DbTransaction transaction, Func<TDataContext, Table<TTable>, DbTransaction, TResult> action)
        {
            var table = context.GetTable<TTable>();
            return Task.Run(() =>
            {
                if (context.Connection.State != ConnectionState.Open)
                    context.Connection.Open();
                context.Transaction = transaction;
                try
                {
                    var res = action(context, table, transaction);
                    context.SubmitChanges();
                    return res;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            });
        }


        /// <summary>
        /// Execute an action on the targeted SQL table without returning a value
        /// The action will be wrapped with a transaction 
        /// The caller is reasonable for calling transaction.Commit
        /// </summary>
        /// <param name="action"></param>
        protected async Task TransactionalExecute(Action<TDataContext, Table<TTable>, DbTransaction> action)
        {
            using (var context = _dbManager.CreateContext())
            {
                var transaction = CreateTransaction(context);
                await TransactionalExecute(context, transaction, action);
            }
        }

        /// <summary>
        /// Execute an action on the database context without returning a value
        /// The action will be wrapped with a transaction 
        /// The caller is reasonable for calling transaction.Commit
        /// </summary>
        protected Task TransactionalExecute(TDataContext context, DbTransaction transaction, Action<TDataContext, Table<TTable>, DbTransaction> action)
        {
            return Task.Run(() =>
            {
                var table = context.GetTable<TTable>();
                context.Transaction = transaction;
                action(context, table, transaction);
                context.SubmitChanges();
            });
        }

        /// <summary>
        /// Retrieve an item from SQL server using the table primary key
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pk"></param>
        /// <returns></returns>
        protected TTable GetItemByPrimaryKey(DataContext context, object pk)
        {
            var table = context.GetTable<TTable>();
            var mapping = context.Mapping.GetTable(typeof(TTable));
            var pkfield = mapping.RowType.DataMembers.SingleOrDefault(d => d.IsPrimaryKey);
            if (pkfield == null)
            {
                pkfield = mapping.RowType.DataMembers.SingleOrDefault(d => d.Name == "ID");
                if (pkfield == null)
                    throw new Exception($"Table {mapping.TableName} does not contain a Primary Key field");
            }
            var param = Expression.Parameter(typeof(TTable), "e");
            var predicate = Expression.Lambda<Func<TTable, bool>>(
                                                                  Expression.Equal(Expression.Property(param,
                                                                                                       pkfield.Name),
                                                                                   Expression.Constant(pk)),
                                                                  param);
            var o = table.SingleOrDefault(predicate);
            return o;
        }
    }

    public class LinqRepositoryBase<TDataContext, TTable, TModel, TKey> :
        LinqRepositoryBase<TDataContext, TTable> where TDataContext : DataContext
        where TTable : class, new()
        where TModel : IUniqueObject<TKey>, new()
    {
        private readonly string _identifier;
        public event EventHandler<RepositoryChangeEventArgs<TModel>> ItemsChanged;

        public LinqRepositoryBase()
        {
            _identifier = typeof(TModel).Name;
        }

        /// <summary>
        /// Called when the <see cref="IRepositorySyncService"/> receives a sync request
        /// </summary>
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

        /// <summary>
        /// Calls the <see cref="IRepository.SetSyncService"/> on the registered repositories
        /// </summary>
        /// <param name="syncService">The sync service to use</param>
        /// <param name="sendSyncRequests">if set to true the repository will send sync requests when insert, edit and delete</param>
        public override void SetSyncService(IRepositorySyncService syncService, bool sendSyncRequests = true)
        {
            base.SetSyncService(syncService, sendSyncRequests);
            ItemsChanged += OnItemsChanged;
        }

        /// <summary>
        /// Default handler for <see cref="ItemsChanged"/> event
        /// </summary>
        private void OnItemsChanged(object sender, RepositoryChangeEventArgs<TModel> e)
        {
            if (!e.FromSync && _sendSyncRequests)
                _syncService?.SyncItem(_identifier, new RepositorySyncEventArgs { Item = e.Item, OperationType = e.OperationType });
        }
    }

    /// <summary>
    /// Base implementation for a LinqToSQL repository that interact with a specific SQL table
    /// Implements the ReadOnly functionality
    /// </summary>
    /// <typeparam name="TDataContext">The linqToSql DataContext</typeparam>
    /// <typeparam name="TTable">The table type</typeparam>
    /// <typeparam name="TModel">The model type</typeparam>
    /// <typeparam name="TKey">The table key(ID) type</typeparam>
    public class ReadOnlyLinqRepositoryBase<TDataContext, TTable, TModel, TKey> :
        LinqRepositoryBase<TDataContext, TTable, TModel, TKey>, IReadOnlyRepository<TModel, TKey>
        where TDataContext : DataContext
        where TTable : class, new()
        where TModel : IUniqueObject<TKey>, new()
    {

        /// <summary>
        /// Register mapping for the table and the model type
        /// </summary>
        public override void RegisterMappings(IObjectMapper mapper)
        {
            mapper.RegisterTwoWayMap<TTable, TModel>();
        }

        /// <summary>
        /// Indexer that retrieve and item from the database by it's primary key (ID)
        /// </summary>
        /// <param name="index">The item key(ID)</param>
        public virtual Task<TModel> this[TKey index]
        {
            get { return Execute((db, table) => _mapper.Map<TModel>(GetItemByPrimaryKey(db, index))); }
        }

        /// <summary>
        /// Return all the items from the repository
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<TModel>> GetItems()
        {
            return GetItemFromDB();
        }

        
        /// <summary>
        /// Execute a function on the targeted SQL table and then returning a value
        /// </summary>
        protected Task<TResult> ExecuteAndMap<TResult>(Func<TDataContext, object> action) where TResult : new()
        {
            var context = _dbManager.CreateContext();
            return Task.Run(() =>
            {
                using (context)
                {
                    var res = _mapper.Map<TResult>(action(context));
                    context.SubmitChanges();
                    return res;
                }
            });
        }

        /// <summary>
        /// Execute a function on the targeted SQL table and then returning a value
        /// </summary>
        protected Task<IEnumerable<TResult>> ExecuteAndMapCollection<TResult>(Func<TDataContext, IList> action) where TResult : new()
        {
            var context = _dbManager.CreateContext();
            return Task.Run(() =>
            {
                using (context)
                {
                    var res = _mapper.MapCollection<TResult>(action(context));
                    context.SubmitChanges();
                    return res;
                }
            });
        }


        /// <summary>
        /// Execute a function on the targeted SQL table and then returning a value
        /// </summary>
        protected Task<TModel> ExecuteAndMap(Func<TDataContext, object> action)
        {
            return ExecuteAndMap<TModel>(action);
        }

        /// <summary>
        /// Execute a function on the targeted SQL table and then returning a value
        /// </summary>
        protected Task<IEnumerable<TModel>> ExecuteAndMapCollection(Func<TDataContext, IList> action)
        {
            return ExecuteAndMapCollection<TModel>(action);
        }

        /// <summary>
        /// Return all the items from the table
        /// </summary>
        /// <returns></returns>
        protected virtual Task<IEnumerable<TModel>> GetItemFromDB()
        {
            return Execute((db, table) => TableToModelMapping(table.ToList()));
        }

        /// <summary>
        /// Calls the MapItemToModel function to convert an item to the model type
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected TModel TableToModelMapping(TTable source)
        {
            return MapItemToModel(source);
        }


        /// <summary>
        /// Calls the MapItemToModel function to convert an item to the model type
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected IEnumerable<TModel> TableToModelMapping(List<TTable> source)
        {
            return MapItemToModel(source);
        }

        /// <summary>
        /// Maps the collection from the table type to the model type
        /// The default behavior is using the Object mapper but can be overridden to alter the behavior
        /// </summary>
        protected virtual IEnumerable<TModel> MapItemToModel(List<TTable> source)
        {
            return _mapper.MapCollection<TModel>(source);
        }

        /// <summary>
        /// Maps the item from the table type to the model type
        /// The default behavior is using the Object mapper but can be overridden to alter the behavior
        /// </summary>
        protected virtual TModel MapItemToModel(TTable source)
        {
            return _mapper.Map<TModel>(source);
        }

        /// <summary>
        /// Maps the item from the table type to the model type
        /// The default behavior is using the Object mapper but can be overridden to alter the behavior
        /// </summary>
        protected virtual void MapItemToModel(TTable source, TModel target)
        {
            _mapper.MapInstance(source, target);
        }
    }

    /// <summary>
    /// Base implementation for a LinqToSQL repository that interact with a specific SQL table
    /// Implements the CRUD(Create,Read,Update,Delete) functionality
    /// </summary>
    /// <typeparam name="TDataContext">The linqToSql DataContext</typeparam>
    /// <typeparam name="TTable">The table type</typeparam>
    /// <typeparam name="TModel">The model type</typeparam>
    /// <typeparam name="TKey">The table key(ID) type</typeparam>
    public class CRUDLinqRepositoryBase<TDataContext, TTable, TModel, TKey> :
        ReadOnlyLinqRepositoryBase<TDataContext, TTable, TModel, TKey>, ICRUDRepository<TModel, TKey>
        where TDataContext : DataContext where TTable : class, new() where TModel : IUniqueObject<TKey>, new()
    {


        /// <summary>
        /// Updates and items in the repository
        /// </summary>
        public virtual Task<TModel> UpdateItem(TModel item, TDataContext context)
        {
            return TransactionalExecute(context, (db, table) =>
              {
                  TTable temp = UpdateToDB(item, db);
                  AditionalUpdate(item, temp, db, db.Transaction);
                  MapItemToModel(temp, item);
                  RaiseItemsChanged(this, new RepositoryChangeEventArgs<TModel>(OperationType.Edit, item, false));
                  return item;
              });
        }
        public virtual async Task<TModel> UpdateItem(TModel item)
        {
            using (var context = _dbManager.CreateContext())
            {
                var transcation = CreateTransaction(context);
                var res= await UpdateItem(item, context);
                transcation.Commit();
                return res;
            }
        }
        protected virtual void AditionalUpdate(TModel item, TTable tableItem, TDataContext db, DbTransaction transaction)
        {

        }

        protected virtual TTable UpdateToDB(TModel item, TDataContext db)
        {
            var temp = GetItemByPrimaryKey(db, item.GetKey);
            MapItemToTable(item, temp);
            db.SubmitChanges();
            return temp;
        }

        /// <summary>
        /// Inserts a new items to the repository
        /// </summary>
        public virtual async Task<TModel> InsertItem(TModel item)
        {
            using (var context = _dbManager.CreateContext())
            {
                var transcation = CreateTransaction(context);
                var res = await InsertItem(item, context);
                transcation.Commit();
                return res;
            }
        }

        /// <summary>
        /// Inserts a new items to the repository
        /// </summary>
        public virtual Task<TModel> InsertItem(TModel item, TDataContext context)
        {
            return TransactionalExecute(context, (db, table) =>
             {
                 TTable tableItem = InsertToDB(item, db);
                 AdditionalInsert(item, tableItem, db, db.Transaction);
                 MapItemToModel(tableItem, item);
                 RaiseItemsChanged(this, new RepositoryChangeEventArgs<TModel>(OperationType.Add, item, false));
                 return item;
             });
        }

        protected virtual void AdditionalInsert(TModel item, TTable tableItem, TDataContext db, DbTransaction transaction)
        {

        }

        protected virtual TTable InsertToDB(TModel item, TDataContext db)
        {
            var tableitem = new TTable();
            MapItemToTable(item, tableitem);
            db.GetTable<TTable>().InsertOnSubmit(tableitem);
            db.SubmitChanges();
            return tableitem;
        }



        /// <summary>
        /// Deletes an item from the repository by it's key
        /// </summary>
        public virtual async Task DeleteItemByKey(TKey ID)
        {
            using (var context = _dbManager.CreateContext())
            {
                CreateTransaction(context);
                await DeleteItemByKey(ID, context);
                context.Transaction.Commit();
            }
        }

        /// <summary>
        /// Deletes an item from the repository by it's key
        /// </summary>
        public virtual async Task DeleteItemByKey(TKey ID, TDataContext context)
        {
           await TransactionalExecute(context, (db, table) =>
           {
               TModel item = DeleteFromDB(ID, db);
               RaiseItemsChanged(this, new RepositoryChangeEventArgs<TModel>(OperationType.Delete, item, false));
           });
        }

        protected virtual TModel DeleteFromDB(TKey ID, TDataContext db)
        {
            var oldItem = GetItemByPrimaryKey(db, ID);
            var item = TableToModelMapping(oldItem);
            db.GetTable<TTable>().DeleteOnSubmit(oldItem);
            db.SubmitChanges();
            return item;
        }



        /// <summary>
        /// Deletes an item from the repository
        /// </summary>
        public virtual async Task DeleteItem(TModel item)
        {
            await DeleteItemByKey(item.GetKey);
        }

        /// <summary>
        /// Deletes an item from the repository
        /// </summary>
        public virtual Task DeleteItem(TModel item, TDataContext context)
        {
            return DeleteItemByKey(item.GetKey, context);
        }
        /// <summary>
        /// Maps a model item to table item the default behavior is using the object mapper
        /// Can be overridden to change the behavior
        /// </summary>
        protected virtual void MapItemToTable(TModel source, TTable destination)
        {
            _mapper.MapInstance(source, destination);
        }

        /// <summary>
        /// Calls ModelToTableMapping Function to map between the model and table types
        /// </summary>
        protected TTable ModelToTableMapping(TModel source)
        {
            var newTableItem = new TTable();
            MapItemToTable(source, newTableItem);
            return newTableItem;
        }
    }
}