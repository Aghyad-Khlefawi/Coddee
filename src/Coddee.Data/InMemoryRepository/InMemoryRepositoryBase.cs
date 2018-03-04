// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Coddee.Data
{
    /// <summary>
    /// Base implementation for an InMemory repository
    /// </summary>
    public class InMemoryRepositoryBase<TModel, TKey> : RepositoryBase<TModel>, IRepository<TModel, TKey> where TModel : IUniqueObject<TKey>
    {
        /// <inheritdoc />
        public override int RepositoryType { get; } = (int)RepositoryTypes.InMemory;

        /// <inheritdoc />
        public override void Initialize(IRepositoryManager repositoryManager,
                                        IObjectMapper mapper,
                                        Type implementedInterface,
                                        RepositoryConfigurations config = null)
        {
            base.Initialize(repositoryManager, mapper, implementedInterface, config);
            _dictionary = new ConcurrentDictionary<TKey, TModel>(Seed().ToDictionary(e => e.GetKey, e => e));
        }

        /// <summary>
        /// The data available in the repository
        /// </summary>
        protected ConcurrentDictionary<TKey, TModel> _dictionary;

        /// <summary>
        /// The data available in the repository
        /// </summary>
        protected IEnumerable<TModel> _collection => _dictionary.Values;

        /// <summary>
        /// Called when the repository content is changed
        /// </summary>
        protected void RaiseItemsChanged(object sender, RepositoryChangeEventArgs<TModel> args)
        {
            ItemsChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Initialize the repository with data.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TModel> Seed()
        {
            return new List<TModel>();
        }

        /// <inheritdoc />
        public event EventHandler<RepositoryChangeEventArgs<TModel>> ItemsChanged;
    }

    /// <summary>
    /// <see cref="IIndexedRepository{TModel,TKey}"/> implementation for in memory repositories.
    /// </summary>
    public class IndexedInMemoryRepositoryBase<TModel, TKey> : InMemoryRepositoryBase<TModel, TKey>, IIndexedRepository<TModel, TKey> where TModel : IUniqueObject<TKey>
    {

        /// <inheritdoc />
        public virtual Task<TModel> this[TKey index]
        {
            get
            {
                _dictionary.TryGetValue(index, out TModel item);
                return Task.FromResult(item);
            }
        }
    }

    /// <summary>
    /// <see cref="IReadOnlyRepository{TModel,TKey}"/> implementation for in memory repositories.
    /// </summary>
    public class ReadOnlyInMemoryRepositoryBase<TModel, TKey> : IndexedInMemoryRepositoryBase<TModel, TKey>, IReadOnlyRepository<TModel, TKey> where TModel : IUniqueObject<TKey>
    {
        /// <inheritdoc />
        public virtual Task<IEnumerable<TModel>> GetItems()
        {
            return Task.FromResult(_collection);
        }
    }

    /// <summary>
    /// <see cref="ICRUDRepository{TModel,TKey}"/> implementation for in memory repositories.
    /// </summary>
    public class CRUDInMemoryRepositoryBase<TModel, TKey> : ReadOnlyInMemoryRepositoryBase<TModel, TKey>, ICRUDRepository<TModel, TKey> where TModel : IUniqueObject<TKey>
    {
        /// <inheritdoc />
        public virtual Task<TModel> UpdateItem(TModel item)
        {
            _dictionary.TryRemove(item.GetKey, out TModel _);
            _dictionary.TryAdd(item.GetKey, item);
            RaiseItemsChanged(this, new RepositoryChangeEventArgs<TModel>(OperationType.Edit, item));
            return Task.FromResult(item);
        }

        /// <inheritdoc />
        public virtual Task<TModel> InsertItem(TModel item)
        {
            _dictionary.TryAdd(item.GetKey, item);
            RaiseItemsChanged(this, new RepositoryChangeEventArgs<TModel>(OperationType.Add, item));
            return Task.FromResult(item);
        }

        /// <inheritdoc />
        public virtual Task DeleteItemByKey(TKey ID)
        {
            _dictionary.TryRemove(ID, out TModel removed);
            RaiseItemsChanged(this, new RepositoryChangeEventArgs<TModel>(OperationType.Delete, removed));
            return Task.FromResult(removed);
        }

        /// <inheritdoc />
        public virtual Task DeleteItem(TModel item)
        {
            return DeleteItemByKey(item.GetKey);
        }
    }
}
