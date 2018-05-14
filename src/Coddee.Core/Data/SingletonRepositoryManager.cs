// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coddee.Attributes;

namespace Coddee.Data
{
    /// <summary>
    /// A repository manager that return the same instance of the repository.
    /// </summary>
    public class SingletonRepositoryManager : RepositoryManagerBase
    {
        /// <inheritdoc/>
        public SingletonRepositoryManager()
        {
            _repositories = new Dictionary<Type, IRepository>();
        }

        /// <summary>
        /// The registered repositories
        /// </summary>
        protected Dictionary<Type, IRepository> _repositories;

        /// <inheritdoc />
        public override IRepository GetRepository(string name)
        {
            foreach (var repository in _repositories)
            {
                if (CheckRepositoryName(repository.Value.GetType(), name))
                    return repository.Value;
            }

            return null;
        }

        /// <summary>
        /// Gets a repository by its interface
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        public override TInterface GetRepository<TInterface>()
        {
            if (!_repositories.ContainsKey(typeof(TInterface)))
                throw new ArgumentException(
                    $"Repository of type {typeof(TInterface).Name} was not found, make sure that the repository is registered and marked with the RepositoryAttribute");
            return (TInterface)_repositories[typeof(TInterface)];
        }

        /// <summary>
        /// Returns all the registered repositories
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IRepository> GetRepositories()
        {
            return _repositories.Values;
        }



        /// <summary>
        /// Register the provided repositories
        /// </summary>
        /// <param name="repositories">The repositories to register</param>
        public override void RegisterRepositories(params KeyValuePair<Type, Type>[] repositories)
        {
            foreach (var repository in repositories)
            {
                var typeInfo = repository.Value.GetTypeInfo();
                if (typeInfo.ImplementedInterfaces.Any(e => e == typeof(IRepository)))
                {
                    if (typeInfo.ImplementedInterfaces.All(e => e != repository.Key))
                        throw new ArgumentException(
                            $"The type {repository.Value.FullName} doesn't implements '{repository.Key.FullName}' interface");

                    bool hasDefaultConstructor = false;
                    foreach (var constructor in typeInfo.DeclaredConstructors)
                    {
                        if (!constructor.IsPublic)
                            continue;
                        var param = constructor.GetParameters();
                        if (param == null || !param.Any())
                            hasDefaultConstructor = true;
                    }

                    if (!hasDefaultConstructor)
                        throw new Exception($"The type {repository.Value.FullName} doesn't have a public parameterless constructor.");
                    IRepository repo;
                    try
                    {
                        repo = (IRepository)Activator.CreateInstance(repository.Value);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"The type {repository.Value.FullName} failed to initialize.", e);
                    }
                    InitializeRepository(repo, repository.Key);
                    AddRepository(repo, repository.Key);
                }
                else
                    throw new ArgumentException(
                        $"The type {repository.Value.FullName} doesn't implements 'ILinqRepository<TDataContext>' interface");
            }
        }

        /// <summary>
        /// Add an <see cref="IRepository"/> to the available repositories
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="implementedRepository"></param>
        public override void AddRepository(IRepository repository, Type implementedRepository)
        {
            _repositories[implementedRepository] = repository;
            if (_syncService != null)
                repository.SetSyncService(_syncService);
        }

        /// <summary>
        /// Calls the <see cref="IRepository.SetSyncService"/> on the registered repositories
        /// </summary>
        /// <param name="syncService">The sync service to use</param>
        /// <param name="sendSyncRequests">if set to true the repositories will send sync requests when insert, edit and delete</param>
        public override void SetSyncService(IRepositorySyncService syncService, bool sendSyncRequests = true)
        {
            base.SetSyncService(syncService, sendSyncRequests);
            foreach (var repo in _repositories.Values)
            {
                repo.SetSyncService(_syncService, sendSyncRequests);
            }
        }



    }
}