// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Threading.Tasks;
using Coddee.Collections;

namespace Coddee.Data
{
    public static class Extensions
    {
        /// <summary>
        /// Adds, edits or delete an item from a repository based on an <see cref="OperationType"/>
        /// </summary>
        /// <typeparam name="TModel">The model object Type</typeparam>
        /// <typeparam name="TKey">The model object key Type</typeparam>
        /// <param name="repo">The target repository</param>
        /// <param name="op">Operation type</param>
        /// <param name="item">The model object</param>
        public static Task<TModel> Update<TModel, TKey>(this ICRUDRepository<TModel,TKey> repo, OperationType op, TModel item) where TModel : IUniqueObject<TKey>
        {
            return op == OperationType.Add ? repo.InsertItem(item) : repo.UpdateItem(item);
        }

        /// <summary>
        /// Adds, edits or delete an item from a repository based on an editor save operation result <see cref="EditorSaveArgs{TModel}"/>
        /// </summary>
        /// <typeparam name="TModel">The model object Type</typeparam>
        /// <typeparam name="TKey">The model object key Type</typeparam>
        /// <param name="repo">The target repository</param>
        /// <param name="args">The editor save arguments</param>
        public static Task<TModel> Update<TModel, TKey>(this ICRUDRepository<TModel, TKey> repo, EditorSaveArgs<TModel> args) where TModel : IUniqueObject<TKey>
        {
            return repo.Update(args.OperationType, args.Item);
        }

        /// <summary>
        /// Adds, edits or delete an item from a repository based on an editor save operation result <see cref="EditorSaveArgs{TModel}"/>
        /// </summary>
        /// <typeparam name="TModel">The model object Type</typeparam>
        /// <typeparam name="TKey">The model object key Type</typeparam>
        /// <param name="repo">The target repository</param>
        /// <param name="args">The editor save arguments</param>
        public static void Update<TModel, TKey>(this ICRUDRepository<TModel, TKey> repo,object sender, EditorSaveArgs<TModel> args) where TModel : IUniqueObject<TKey>
        {
            repo.Update(args.OperationType, args.Item);
        }

        /// <summary>
        /// Updates the collection base on the operation type 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Update<T>(this IList<T> collection, object sender, RepositoryChangeEventArgs<T> args)
        {
            collection.Update(args.OperationType,args.Item);
        }

        /// <summary>
        /// Updates the collection base on save arguments
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Update<T>(this IList<T> collection, object sender, EditorSaveArgs<T> args)
        {
            collection.Update(args.OperationType, args.Item);
        }


        /// <summary>
        /// Updates the collection base on the operation type 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void BindToRepositoryChanges<T, TKey>(this IList<T> collection, ICRUDRepository<T,TKey> repo) 
            where T : IUniqueObject<TKey>
        {
            repo.ItemsChanged += collection.Update;
        }

        public static async Task<AsyncObservableCollection<T>> ToAsyncObservableCollection<T, TKey>(this ICRUDRepository<T, TKey> repo)
            where T : IUniqueObject<TKey>
        {
            var items = await repo.GetItems();
            var collection = AsyncObservableCollection<T>.Create(items);
            collection.BindToRepositoryChanges(repo);
            return collection;
        }
    }
}
