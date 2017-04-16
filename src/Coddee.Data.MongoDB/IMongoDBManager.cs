// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using MongoDB.Driver;

namespace Coddee.Data.MongoDB
{
    /// <summary>
    /// Defines the requirements for MongoDBManager
    /// The DBManager holds the connection to the database and creates clients to connect to the database
    /// </summary>
    public interface IMongoDBManager
    {
        /// <summary>
        /// Creates a new IMongoClient object
        /// </summary>
        IMongoClient CreateNewClient();

        /// <summary>
        /// Returns a single instance of IMongoClient object
        /// </summary>
        IMongoClient GetClient();

        /// <summary>
        /// Returns the IMongoDatabase associated with this DBManager
        /// </summary>
        IMongoDatabase GetDatabase();
    }
}
