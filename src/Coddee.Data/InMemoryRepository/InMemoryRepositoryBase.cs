// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coddee.Data
{
    public class InMemoryRepositoryBase<TModel, TKey> : RepositoryBase, IRepository<TModel, TKey> where TModel : IUniqueObject<TKey>
    {
        public override int RepositoryType { get; } = (int)RepositoryTypes.InMemory;

        public InMemoryRepositoryBase()
        {
            _dictionary = new ConcurrentDictionary<TKey, TModel>();
        }

        protected ConcurrentDictionary<TKey, TModel> _dictionary;
        protected IEnumerable<TModel> _collection => _dictionary.Values;

        protected void RaiseItemsChanged(object sender, RepositoryChangeEventArgs<TModel> args)
        {
            ItemsChanged?.Invoke(this, args);
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
