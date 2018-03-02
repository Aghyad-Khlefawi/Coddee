// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using MongoDB.Driver;

namespace Coddee.Data.MongoDB
{
    /// <summary>
    /// Defines the requirements for MongoDBManager
    /// The DBManager holds the connection to the database and creates clients to connect to the database
    /// </summary>
    public class MongoDBManager:IMongoDBManager
    {
        private readonly string _connection;
        private readonly string _dbName;
        private IMongoClient _client;

        /// <inheritdoc />
        public MongoDBManager(string connection,string dbName)
        {
            _connection = connection;
            _dbName = dbName;
        }

        /// <summary>
        /// Creates a new IMongoClient object
        /// </summary>
        public virtual IMongoClient CreateNewClient()
        {
            return new MongoClient(_connection);
        }

        /// <summary>
        /// Returns a single instance of IMongoClient object
        /// </summary>
        public virtual IMongoClient GetClient()
        {
            return _client ?? (_client = CreateNewClient());
        }


        /// <summary>
        /// Returns the IMongoDatabase associated with this DBManager
        /// </summary>
        public virtual IMongoDatabase GetDatabase()
        {
            return GetClient().GetDatabase(_dbName);
        }
    }
}
