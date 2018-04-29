using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Coddee.Data.SQLite
{
    public class SQLiteRepositoryBase : RepositoryBase, ISQLiteRepository
    {
        public override int RepositoryType { get; } = (int) RepositoryTypes.SQLite;

        protected SQLiteDBManager _dbManager;


        public virtual void Initialize(ISQLiteDBManager dbManager, IRepositoryManager repositoryManager,
            IObjectMapper mapper,
            Type implementedInterface, RepositoryConfigurations config = null)
        {
            _dbManager = (SQLiteDBManager) dbManager;
            Initialize(repositoryManager, mapper, implementedInterface, config);
        }

        /// <summary>
        /// Execute an action on the database without returning a value
        /// </summary>
        protected Task Execute(Action<SQLiteConnection> action)
        {
            return Task.Run(() =>
            {
                using (var connection = _dbManager.CreateConnection())
                {
                    action(connection);
                }
            });
        }

        /// <summary>
        /// Execute an task on the database without returning a value
        /// </summary>
        protected async Task ExecuteAsync(Func<SQLiteAsyncConnection, Task> action)
        {
            var connection = _dbManager.CreateConnectionAsync();
            await action(connection);
        }

        /// <summary>
        /// Execute a function on the database and then return a value
        /// </summary>
        protected Task<TResult> Execute<TResult>(Func<SQLiteConnection, TResult> action)
        {
            return Task.Run(() =>
            {
                using (var connection = _dbManager.CreateConnection())
                {
                    var res = action(connection);
                    return res;
                }
            });
        }

        /// <summary>
        /// Execute a function on the database context and then return a value
        /// </summary>
        protected async Task<TResult> ExecuteAsync<TResult>(Func<SQLiteAsyncConnection, Task<TResult>> action)
        {
            var connection = _dbManager.CreateConnectionAsync();
            return await action(connection);
        }
    }
}