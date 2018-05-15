// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coddee.Collections;

namespace Coddee.Data
{
    /// <summary>
    /// Extension methods for data related classes.
    /// </summary>
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
        public static Task<TModel> Update<TModel, TKey>(this ICRUDRepository<TModel, TKey> repo, OperationType op, TModel item) where TModel : IUniqueObject<TKey>
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
        /// <param name="sender">the repository</param>
        /// <param name="args">The editor save arguments</param>
        public static void Update<TModel, TKey>(this ICRUDRepository<TModel, TKey> repo, object sender, EditorSaveArgs<TModel> args) where TModel : IUniqueObject<TKey>
        {
            repo.Update(args.OperationType, args.Item);
        }

        /// <summary>
        /// Updates the collection base on the operation type 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Update<T>(this IList<T> collection, object sender, RepositoryChangeEventArgs<T> args)
        {
            collection.Update(args);
        }

        /// <summary>
        /// Updates the collection base on the operation type 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Update<T>(this IList<T> collection, RepositoryChangeEventArgs<T> args)
        {
            collection.Update(args.OperationType, args.Item);
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
        public static void BindToRepositoryChanges<T, TKey>(this IList<T> collection, ICRUDRepository<T, TKey> repo)
            where T : IUniqueObject<TKey>
        {
            repo.ItemsChanged += collection.Update;
        }

        /// <summary>
        /// Updates the collection base on the operation type 
        /// </summary>
        public static void BindToRepositoryChanges<T, TKey>(this IList<T> collection, ICRUDRepository<T, TKey> repo,Func<T,bool> predicate)
            where T : IUniqueObject<TKey>
        {
            repo.ItemsChanged += (s, e) =>
            {
                if (predicate(e.Item))
                    collection.Update(e);
            };
        }

        /// <summary>
        /// Updates the collection base on the operation type 
        /// </summary>
        public static void BindToRepositoryChanges<T, TKey, TTarget>(this IList<TTarget> collection, ICRUDRepository<T, TKey> repo, Projection<T, TTarget> porjection, ItemLocator<T, TTarget> locator)
            where T : IUniqueObject<TKey>
        {
            repo.ItemsChanged += (s, e) =>
            {
                var target = porjection(e.Item);
                UpdateCollectionOnRepositoryChange<T, TKey, TTarget>(collection, locator, e, target);
            };
        }

        /// <summary>
        /// Updates the collection base on the operation type 
        /// </summary>
        public static void BindToRepositoryChangesAsync<T, TKey, TTarget>(this IList<TTarget> collection, ICRUDRepository<T, TKey> repo, Projection<T, Task<TTarget>> porjection, ItemLocator<T, TTarget> locator)
            where T : IUniqueObject<TKey>
        {
            repo.ItemsChanged += async (s, e) =>
            {
                var target = await porjection(e.Item);
                UpdateCollectionOnRepositoryChange<T, TKey, TTarget>(collection, locator, e, target);
            };
        }

        private static void UpdateCollectionOnRepositoryChange<T, TKey, TTarget>(IList<TTarget> collection, ItemLocator<T, TTarget> locator, RepositoryChangeEventArgs<T> e, TTarget target) where T : IUniqueObject<TKey>
        {

            if (e.OperationType == OperationType.Add)
                collection.Add(target);
            else
            {
                var original = locator(e.Item);
                if (e.OperationType == OperationType.Edit)
                    collection.Update(target, p => p.Equals(original));
                else if (e.OperationType == OperationType.Delete)
                    collection.Remove(original);
            }
        }

        /// <summary>
        /// Creates an <see cref="AsyncObservableCollection{T}"/> and binds the repository changes to the collection
        /// </summary>
        public static async Task<AsyncObservableCollection<T>> ToAsyncObservableCollection<T, TKey>(this ICRUDRepository<T, TKey> repo)
            where T : IUniqueObject<TKey>
        {
            var items = await repo.GetItems();
            var collection = AsyncObservableCollection<T>.Create(items);
            collection.BindToRepositoryChanges(repo);
            return collection;
        }

        /// <summary>
        /// Projects each element of a task sequence result into a new form
        /// </summary>
        public static async Task<IEnumerable<TResult>> Select<T, TResult>(this Task<IEnumerable<T>> collection, Func<T, TResult> func)
        {
            return (await collection).Select(func);
        }
    }
}
