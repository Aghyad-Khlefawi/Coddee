// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Coddee.Data
{
    /// <summary>
    /// Manages the storing of data for FileRepositories.
    /// </summary>
    public class DataFile
    {
        /// <inheritdoc />
        public DataFile(string fileName, string fileDirectory, JsonSerializer jsonSerializer)
        {
            _fileName = fileName;
            _fileDirectory = fileDirectory;
            _jsonSerializer = jsonSerializer;
        }

        /// <summary>
        /// The file name.
        /// </summary>
        protected readonly string _fileName;

        /// <summary>
        /// The path to the directory that contains the file.
        /// </summary>
        protected readonly string _fileDirectory;

        /// <summary>
        /// The json serializer used to serialize and deserialize the data.
        /// </summary>
        private readonly JsonSerializer _jsonSerializer;

        /// <summary>
        /// The default bson reader instance.
        /// </summary>
        protected BsonReader _bsonReader;

        /// <summary>
        /// The default bson writer instance.
        /// </summary>
        protected BsonWriter _bsonWriter;

        /// <summary>
        /// The file stream to the bson file.
        /// </summary>
        protected FileStream _fileStream;


        /// <summary>
        /// Returns a <see cref="FileInfo"/> object for the file.
        /// </summary>
        /// <returns></returns>
        public virtual FileInfo GetFile()
        {
            var name = $"{_fileName}.bson";
            if (!Directory.Exists(_fileDirectory))
                Directory.CreateDirectory(_fileDirectory);

            var file = new FileInfo(Path.Combine(_fileDirectory, name));
            if (!file.Exists)
                file.Create().Dispose();
            return file;
        }

        /// <summary>
        /// Creates or returns a file stream.
        /// </summary>
        /// <returns></returns>
        protected virtual FileStream GetFileStream()
        {
            if (_fileStream == null)
                _fileStream = GetFile().Open(FileMode.Open, FileAccess.ReadWrite);
            return _fileStream;
        }

        /// <summary>
        /// Write the collection to the disk.
        /// </summary>
        /// <returns></returns>
        public virtual async Task UpdateFile(object data)
        {
            if (_bsonWriter == null)
            {
                _bsonWriter = new BsonWriter(GetFileStream());
            }
            _jsonSerializer.Serialize(_bsonWriter, data);
            await _fileStream.FlushAsync();
        }

        /// <summary>
        /// Reads and deserializes the content of the file.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public virtual async Task<TResult> ReadFile<TResult>()
        {
            return await Task.Run(() =>
            {
                if (_bsonReader == null)
                {
                    _bsonReader = CreateBsonReader();
                }
                return _jsonSerializer.Deserialize<TResult>(_bsonReader);
            });
        }

        /// <summary>
        /// Create BsonReader instance.
        /// </summary>
        /// <returns></returns>
        protected virtual BsonReader CreateBsonReader()
        {
            return new BsonReader(GetFileStream());
        }
    }

    /// <summary>
    /// Extends <see cref="DataFile"/> and keeps a local version of the data.
    /// </summary>
    public abstract class DataFile<T> : DataFile where T : IEnumerable
    {
        /// <inheritdoc />
        protected DataFile(string fileName, string fileDirectory, JsonSerializer jsonSerializer) : base(fileName, fileDirectory, jsonSerializer)
        {

        }

        /// <summary>
        /// Indicates that the file data was read of not.
        /// </summary>
        protected bool _fileRead;

        /// <summary>
        /// A local collection of the data available in the file.
        /// </summary>
        protected T _items;

        private async Task ReadFile()
        {
            if (!_fileRead)
            {
                var data = await ReadFile<T>();
                _items = SetDefaultCollectionValueOnRead(data);
                _fileRead = true;
            }
        }

        /// <summary>
        /// Return default value for the stored type.
        /// </summary>
        /// <param name="data">The value returned from the file.</param>
        /// <returns></returns>
        protected abstract T SetDefaultCollectionValueOnRead(T data);

        /// <summary>
        /// Write the collection to the disk.
        /// </summary>
        public virtual Task UpdateFile()
        {
            return UpdateFile(_items);
        }

        /// <summary>
        /// Return the local collection.
        /// </summary>
        public virtual async Task<T> GetCollection()
        {
            await ReadFile();
            return _items;
        }
    }

    /// <summary>
    /// Extends <see cref="DataFile"/> and keeps a local version of the data.
    /// </summary>
    public class RepositoryDataFile<TKey, TModel> : DataFile<ConcurrentDictionary<TKey, TModel>>
    {
        /// <inheritdoc />
        public RepositoryDataFile(string fileName, string fileDirectory, JsonSerializer jsonSerializer) : base(fileName, fileDirectory, jsonSerializer)
        {

        }

        protected override ConcurrentDictionary<TKey, TModel> SetDefaultCollectionValueOnRead(ConcurrentDictionary<TKey, TModel> data)
        {
            return data ?? new ConcurrentDictionary<TKey, TModel>();
        }
    }

}