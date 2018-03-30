
namespace Coddee.Data.SQLite
{
    /// <summary>
    /// An object responsible for creating an setting the SQLiteConnection object.
    /// </summary>
    public interface ISQLiteDBManager
    {
        /// <summary>
        /// The database connection string.
        /// </summary>
        string Connection { get; set; }

        /// <summary>
        /// Initialize the DbManager
        /// </summary>
        /// <param name="connection">The database connection string.</param>
        void Initialize(string connection);
    }
}
