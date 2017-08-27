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
        public override int RepositoryType { get; } = (int)RepositoryTypes.InMemory;

        public override void Initialize(IRepositoryManager repositoryManager,
                                        IObjectMapper mapper,
                                        Type implementedInterface,
                                        RepositoryConfigurations config = null)
        {
            base.Initialize(repositoryManager, mapper, implementedInterface, config);
            _dictionary = new ConcurrentDictionary<TKey, TModel>(Seed().ToDictionary(e => e.GetKey, e => e));
        }

        protected ConcurrentDictionary<TKey, TModel> _dictionary;
        protected IEnumerable<TModel> _collection => _dictionary.Values;

        protected void RaiseItemsChanged(object sender, RepositoryChangeEventArgs<TModel> args)
        {
            ItemsChanged?.Invoke(this, args);
        }

        public virtual IEnumerable<TModel> Seed()
        {
            return new List<TModel>();
        }

        public event EventHandler<RepositoryChangeEventArgs<TModel>> ItemsChanged;
    }

    public class IndexedInMemoryRepositoryBase<TModel, TKey> : InMemoryRepositoryBase<TModel, TKey>, IIndexedRepository<TModel, TKey> where TModel : IUniqueObject<TKey>
    {

        public virtual Task<TModel> this[TKey index]
        {
            get
            {
                _dictionary.TryGetValue(index, out TModel item);
                return Task.FromResult(item);
            }
        }
    }

    public class ReadOnlyInMemoryRepositoryBase<TModel, TKey> : IndexedInMemoryRepositoryBase<TModel, TKey>, IReadOnlyRepository<TModel, TKey> where TModel : IUniqueObject<TKey>
    {
        public virtual Task<IEnumerable<TModel>> GetItems()
        {
            return Task.FromResult(_collection);
        }

        public Task<IEnumerable<TModel>> GetItems<T>(params Condition<TModel, T>[] conditions)
        {
            var query = _collection.AsQueryable();
            query = BuildConditionQuery(conditions, query);
            return Task.FromResult(query.ToList().AsEnumerable());
        }

        protected static IQueryable<TModel> BuildConditionQuery<T>(Condition<TModel, T>[] conditions, IQueryable<TModel> query)
        {
            foreach (var condition in conditions)
            {
                var param = Expression.Parameter(typeof(TModel), "e");
                var value = Expression.Constant(condition.Value);

                var propertyName = ((MemberExpression)condition.Property.Body).Member.Name;
                var property = typeof(TModel).GetTypeInfo().GetProperty(propertyName);
                if (property == null)
                    throw new ArgumentException($"There is no property named {propertyName} on type {typeof(TModel).FullName}");

                var prop = Expression.MakeMemberAccess(param, property);
                var body = Expression.Equal(prop, value);
                var expressions = Expression.Lambda<Func<TModel, bool>>(body, param);
                query = query.Where(expressions);
            }

            return query;
        }
    }

    public class CRUDInMemoryRepositoryBase<TModel, TKey> : ReadOnlyInMemoryRepositoryBase<TModel, TKey>, ICRUDRepository<TModel, TKey> where TModel : IUniqueObject<TKey>
    {
        public virtual Task<TModel> UpdateItem(TModel item)
        {
            _dictionary.TryRemove(item.GetKey, out TModel _);
            _dictionary.TryAdd(item.GetKey, item);
            RaiseItemsChanged(this, new RepositoryChangeEventArgs<TModel>(OperationType.Edit, item));
            return Task.FromResult(item);
        }

        public virtual Task<TModel> InsertItem(TModel item)
        {
            _dictionary.TryAdd(item.GetKey, item);
            RaiseItemsChanged(this, new RepositoryChangeEventArgs<TModel>(OperationType.Add, item));
            return Task.FromResult(item);
        }

        public virtual Task DeleteItem(TKey ID)
        {
            _dictionary.TryRemove(ID, out TModel removed);
            RaiseItemsChanged(this, new RepositoryChangeEventArgs<TModel>(OperationType.Delete, removed));
            return Task.FromResult(removed);
        }

        public virtual Task DeleteItem(TModel item)
        {
            return DeleteItem(item.GetKey);
        }
    }
}
