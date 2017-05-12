// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Data.Linq;

namespace Coddee.Data.LinqToSQL
{
    public interface ILinqDBManager
    {
        string Connection { get; set; }
        void Initialize(string connection);
    }

    /// <summary>
    /// This class holds the database connection string and responsible for creating the DataContext objects
    /// </summary>
    public abstract class LinqDBManager<TDataContext>: ILinqDBManager where TDataContext : DataContext
    {
        public string Connection { get; private set; }

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