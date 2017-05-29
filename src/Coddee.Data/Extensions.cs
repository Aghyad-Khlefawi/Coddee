using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.Data
{
    public static class Extensions
    {
        public static Task<TModel> Update<TModel, TKey>(this ICRUDRepository<TModel,TKey> repo, OperationType op, TModel item) where TModel : IUniqueObject<TKey>
        {
            return op == OperationType.Add ? repo.InsertItem(item) : repo.UpdateItem(item);
        }
        public static Task<TModel> Update<TModel, TKey>(this ICRUDRepository<TModel, TKey> repo, EditorSaveArgs<TModel> args) where TModel : IUniqueObject<TKey>
        {
            return repo.Update(args.OperationType, args.Item);
        }
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
        /// Updates the collection base on the operation type 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void BindToRepositoryChanges<T, TKey>(this IList<T> collection, ICRUDRepository<T,TKey> repo) 
            where T : IUniqueObject<TKey>
        {
            repo.ItemsChanged += collection.Update;
        }
    }
}
