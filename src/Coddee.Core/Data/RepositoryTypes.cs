// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Data
{
    /// <summary>
    /// Types of repositories supported
    /// </summary>
    public enum RepositoryTypes
    {
        /// <summary>
        /// A repository that keeps the data in memory.
        /// </summary>
        InMemory,

        /// <summary>
        /// A repository that stores the data in an SQL server using LinqToSql
        /// </summary>
        Linq,

        /// <summary>
        /// A repository that stores the data in an SQL server using a REST API
        /// </summary>
        REST,

        /// <summary>
        /// A repository that stores the data in a Mongo database using MongoDB client
        /// </summary>
        Mongo,
        
        /// <summary>
        /// A repository that stores the data in a SQLite database using SQLite-Net
        /// </summary>
        SQLite
    }
}
