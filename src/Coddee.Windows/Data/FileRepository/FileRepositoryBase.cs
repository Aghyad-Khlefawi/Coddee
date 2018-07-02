// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Coddee.Data
{
    public interface IFileRepository : IRepository
    {
        /// <summary>
        /// Do any required initialization
        /// </summary>
        void Initialize(string repositoriesDirectory, IRepositoryManager repositoryManager, IObjectMapper mapper, Type implementedInterface, RepositoryConfigurations config = null);
    }

    /// <summary>
    /// Repository implementation that stores the data on files on the system.
    /// Used for small amount of records.
    /// </summary>
    public abstract class FileRepositoryBase<TModel, TKey> : RepositoryBase<TModel>, IFileRepository, IRepository<TModel, TKey> where TModel : IUniqueObject<TKey>
    {

        protected FileRepositoryBase(string fileName)
        {
            _fileName = fileName;
            _dataFiles = new Dictionary<string, DataFile>();
        }

        /// <summary>
        /// The data files used to store the repository data.
        /// </summary>
        protected Dictionary<string, DataFile> _dataFiles;

        /// <summary>
        /// The data file that contains the main repository data.
        /// </summary>
        protected RepositoryDataFile<TKey, TModel> DefaultRepositoryDataFile;

        /// <inheritdoc />
        public override int RepositoryType { get; } = (int)RepositoryTypes.File;

        /// <summary>
        /// The default json serializer instance.
        /// </summary>
        protected JsonSerializer _jsonSerializer;

        /// <summary>
        /// Cache collection for items in the file.
        /// </summary>
        protected Task<ConcurrentDictionary<TKey, TModel>> GetCollection()
        {
            return DefaultRepositoryDataFile.GetCollection();
        }



        /// <summary>
        /// The path to the directory containing the repository files.
        /// </summary>
        protected string _repositoriesDirectory;


        /// <summary>
        /// The name of the file containing the records.
        /// </summary>
        protected readonly string _fileName;



        ///<inheritdoc/>
        public void Initialize(string repositoriesDirectory, IRepositoryManager repositoryManager, IObjectMapper mapper, Type implementedInterface, RepositoryConfigurations config = null)
        {
            base.Initialize(repositoryManager, mapper, implementedInterface, config);
            _repositoriesDirectory = repositoriesDirectory;
            _jsonSerializer = JsonSerializer.Create();
            CreateDataFiles();
        }

        /// <summary>
        /// Create the data file required by the repository.
        /// </summary>
        protected virtual void CreateDataFiles()
        {
            DefaultRepositoryDataFile = CreateDataFile<TKey, TModel>(_fileName);
        }

        /// <summary>
        /// Creates a new <see cref="DataFile"/>
        /// </summary>
        protected DataFile CreateDataFile(string fileName)
        {
            var file = new DataFile(fileName, _repositoriesDirectory, _jsonSerializer);
            _dataFiles[fileName] = file;
            return file;
        }

        /// <summary>
        /// Creates a new <see cref="DataFile"/>
        /// </summary>
        protected RepositoryDataFile<TFKey, TFModel> CreateDataFile<TFKey, TFModel>(string fileName)
        {
            var file = new RepositoryDataFile<TFKey, TFModel>(fileName, _repositoriesDirectory, _jsonSerializer);
            _dataFiles[fileName] = file;
            return file;
        }

        /// <summary>
        /// Retrieve a <see cref="DataFile"/>
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="createIfNotExists"></param>
        /// <returns></returns>
        protected DataFile GetDataFile(string fileName, bool createIfNotExists)
        {
            if (!_dataFiles.ContainsKey(fileName))
            {
                if (createIfNotExists)
                    return CreateDataFile(fileName);
                return null;
            }
            return _dataFiles[fileName];
        }


        /// <summary>
        /// Write the collection to the disk.
        /// </summary>
        /// <returns></returns>
        public virtual Task UpdateFile()
        {
            return DefaultRepositoryDataFile.UpdateFile();
        }

        /// <summary>
        /// Read the data from the disk.
        /// </summary>
        /// <param name="readFile">if set to true the data will be updated from the file.
        /// however if the local data is null then the file will be read anyway.</param>
        /// <returns></returns>
        public virtual Task<ConcurrentDictionary<TKey, TModel>> GetRepositoryItems(bool readFile = false)
        {
            return DefaultRepositoryDataFile.GetCollection();
        }



        /// <summary>
        /// Called when the repository content is changed
        /// </summary>
        protected void RaiseItemsChanged(object sender, RepositoryChangeEventArgs<TModel> args)
        {
            ItemsChanged?.Invoke(this, args);
        }

        public event EventHandler<RepositoryChangeEventArgs<TModel>> ItemsChanged;
    }

    /// <summary>
    /// <see cref="IIndexedRepository{TModel,TKey}"/> implementation for file repositories.
    /// </summary>
    public abstract class IndexedFileRepositoryBase<TModel, TKey> : FileRepositoryBase<TModel, TKey>, IIndexedRepository<TModel, TKey> where TModel : IUniqueObject<TKey>
    {
        protected IndexedFileRepositoryBase(string fileName) : base(fileName)
        {
        }

        /// <inheritdoc />
        public virtual Task<TModel> this[TKey index]
        {
            get
            {
                async Task<TModel> GetItemInternal()
                {
                    (await GetRepositoryItems()).TryGetValue(index, out var item);
                    return item;
                }
                return GetItemInternal();
            }
        }

    }

    /// <summary>
    /// <see cref="IReadOnlyRepository{TModel,TKey}"/> implementation for file repositories.
    /// </summary>
    public abstract class ReadOnlyFileRepositoryBase<TModel, TKey> : IndexedFileRepositoryBase<TModel, TKey>, IReadOnlyRepository<TModel, TKey> where TModel : IUniqueObject<TKey>
    {
        protected ReadOnlyFileRepositoryBase(string fileName) : base(fileName)
        {
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TModel>> GetItems()
        {
            return (await GetRepositoryItems()).Values;
        }

    }

    /// <summary>
    /// <see cref="ICRUDRepository{TModel,TKey}"/> implementation for file repositories.
    /// </summary>
    public abstract class CRUDFileRepositoryBase<TModel, TKey> : ReadOnlyFileRepositoryBase<TModel, TKey>, ICRUDRepository<TModel, TKey> where TModel : IUniqueObject<TKey>
    {
        protected CRUDFileRepositoryBase(string fileName) : base(fileName)
        {
        }

        /// <inheritdoc />
        public virtual async Task<TModel> UpdateItem(TModel item)
        {
            var items = await GetCollection();
            items.TryRemove(item.GetKey, out TModel _);
            items.TryAdd(item.GetKey, item);
            await UpdateFile();
            RaiseItemsChanged(this, new RepositoryChangeEventArgs<TModel>(OperationType.Edit, item));
            return item;
        }

        /// <inheritdoc />
        public virtual async Task<TModel> InsertItem(TModel item)
        {
            var items = await GetCollection();
            items.TryAdd(item.GetKey, item);
            await UpdateFile();
            RaiseItemsChanged(this, new RepositoryChangeEventArgs<TModel>(OperationType.Add, item));
            return item;
        }

        /// <inheritdoc />
        public virtual async Task DeleteItemByKey(TKey ID)
        {
            var items = await GetCollection();
            items.TryRemove(ID, out TModel removed);
            await UpdateFile();
            RaiseItemsChanged(this, new RepositoryChangeEventArgs<TModel>(OperationType.Delete, removed));
        }

        /// <inheritdoc />
        public virtual Task DeleteItem(TModel item)
        {
            return DeleteItemByKey(item.GetKey);
        }

    }
}
