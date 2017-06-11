// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coddee.Data
{
    public class RepositoryConfigurations
    {
        public string EncrpyionKey { get; set; }

    }

    /// <summary>
    /// A data repository
    /// </summary>
    public interface IRepository
    {
        bool Initialized { get; }
        Type ImplementedInterface { get; }

        /// <summary>
        /// Do any required initialization
        /// </summary>
        void Initialize(IRepositoryManager repositoryManager,
                        IObjectMapper mapper,
                        Type implementedInterface,
                        RepositoryConfigurations config=null);

        void SetSyncService(IRepositorySyncService syncService);
    }

    /// <summary>
    /// A data repository
    /// </summary>
    public interface IRepository<TModel, TKey>: IRepository
        where TModel : IUniqueObject<TKey>
    {
        event EventHandler<RepositoryChangeEventArgs<TModel>> ItemsChanged;
    }

    /// <summary>
    /// A data repository contains an indexer to retrieve an item by its key
    /// </summary>
    /// <typeparam name="TModel">The model type</typeparam>
    /// <typeparam name="TKey">The Key(ID) Type</typeparam>
    public interface IIndexedRepository<TModel, TKey> : IRepository<TModel, TKey>
        where TModel : IUniqueObject<TKey>
    {


        /// <summary>
        /// Return the item by its key(ID)
        /// </summary>
        /// <param name="index">The item Key(ID)</param>
        /// <returns></returns>
        Task<TModel> this[TKey index] { get; }
    }

    /// <summary>
    /// A data repository for read only functionality
    /// </summary>
    /// <typeparam name="TModel">The model type</typeparam>
    /// <typeparam name="TKey">The Key(ID) Type</typeparam>
    public interface IReadOnlyRepository<TModel, TKey> : IIndexedRepository<TModel, TKey>
        where TModel : IUniqueObject<TKey>

    {
        /// <summary>
        /// Returns all the items in the repository
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TModel>> GetItems();
    }

    /// <summary>
    /// A data repository that support CURD operations(Create,Update,Retrieve,Delete)
    /// </summary>
    /// <typeparam name="TModel">The model type</typeparam>
    /// <typeparam name="TKey">The Key(ID) Type</typeparam>
    public interface ICRUDRepository<TModel, TKey> : IReadOnlyRepository<TModel, TKey>
        where TModel : IUniqueObject<TKey>

    {

        /// <summary>
        /// Updates and items in the repository
        /// </summary>
        Task<TModel> UpdateItem(TModel item);

        /// <summary>
        /// Inserts a new items to the repository
        /// </summary>
        Task<TModel> InsertItem(TModel item);

        /// <summary>
        /// Deletes an item from the repository by it's key
        /// </summary>
        Task DeleteItem(TKey ID);

        /// <summary>
        /// Deletes an item from the repository 
        /// </summary>
        Task DeleteItem(TModel item);
    }
}