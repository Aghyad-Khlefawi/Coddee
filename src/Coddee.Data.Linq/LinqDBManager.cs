// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Data.Linq;

namespace Coddee.Data.LinqToSQL
{
    /// <summary>
    /// An object responsible for creating an setting the connection of DataContext objects.
    /// </summary>
    public interface ILinqDBManager
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

    /// <summary>
    /// This class holds the database connection string and responsible for creating the DataContext objects
    /// </summary>
    public abstract class LinqDBManager<TDataContext>: ILinqDBManager where TDataContext : DataContext
    {
        /// <inheritdoc />
        public string Connection { get; set; }

        /// <inheritdoc />
        public void Initialize(string connection)
        {
            Connection = connection;
        }
        /// <summary>
        /// Creates a new DataContext using the stored connection string (Connection)
        /// </summary>
        /// <returns></returns>
        public abstract TDataContext CreateContext();
    }
}