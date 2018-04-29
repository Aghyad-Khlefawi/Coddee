using SQLite;

namespace Coddee.Data.SQLite
{
    public abstract class SQLiteDBManager : ISQLiteDBManager
    {
        /// <inheritdoc />
        public string Connection { get; set; }

        /// <inheritdoc />
        public void Initialize(string connection)
        {
            Connection = connection;
        }
        
        /// <summary>
        /// Creates a new SQLiteConnection using the stored connection string (Connection)
        /// </summary>
        /// <returns></returns>
        public abstract SQLiteConnection CreateConnection();

        /// <summary>
        /// Creates a new SQLiteAsyncConnection using the stored connection string (Connection)
        /// </summary>
        /// <returns></returns>
        public abstract SQLiteAsyncConnection CreateConnectionAsync();
    }
}
