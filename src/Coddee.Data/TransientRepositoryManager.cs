// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Coddee.Data
{
    /// <summary>
    /// A repository manager that returns a new instance of the repository on each call.
    /// </summary>
    public class TransientRepositoryManager : RepositoryManagerBase
    {
        /// <inheritdoc />
        public TransientRepositoryManager()
        {
            _registeredRepositoryTypes = new Dictionary<Type, Type>();
            _singletonRepositories = new Dictionary<Type, IRepository>();
        }

        /// <summary>
        /// Singelton repository registered with <see cref="AddRepository"/>
        /// </summary>
        private readonly Dictionary<Type, IRepository> _singletonRepositories;
        
        /// <summary>
        /// The supported repository types.
        /// </summary>
        private readonly Dictionary<Type, Type> _registeredRepositoryTypes;

        /// <summary>
        /// Gets a repository by its interface
        /// </summary>
        public IRepository GetRepository(Type interfaceType)
        {
            if (_singletonRepositories.ContainsKey(interfaceType))
                return _singletonRepositories[interfaceType];

            if (!_registeredRepositoryTypes.ContainsKey(interfaceType))
                throw new InvalidOperationException($"Repository of type {interfaceType} is not registered.");

            var implementation = _registeredRepositoryTypes[interfaceType];
            var repo = (IRepository)Activator.CreateInstance(implementation);
            InitializeRepository(repo, interfaceType);

            if (_syncService != null)
                repo.SetSyncService(_syncService, _sendSyncRequests);
            return repo;
        }

        /// <inheritdoc />
        public override IRepository GetRepository(string name)
        {
            foreach (var repository in _registeredRepositoryTypes)
            {
                if (CheckRepositoryName(repository.Value, name))
                    return GetRepository(repository.Key);
            }

            return null;
        }

        /// <inheritdoc />
        public override TInterface GetRepository<TInterface>()
        {
            return (TInterface)GetRepository(typeof(TInterface));
        }

        /// <inheritdoc />
        public override IEnumerable<IRepository> GetRepositories()
        {
            return _registeredRepositoryTypes.Select(e => GetRepository(e.Key)).ToList();
        }

        /// <inheritdoc />
        public override void RegisterRepositories(params KeyValuePair<Type, Type>[] repositories)
        {
            foreach (var repository in repositories)
            {
                if (repository.Value.GetTypeInfo().ImplementedInterfaces.Any(e => e == typeof(IRepository)))
                {
                    if (repository.Value.GetTypeInfo().ImplementedInterfaces.All(e => e != repository.Key))
                        throw new ArgumentException($"The type {repository.Value.FullName} doesn't implements '{repository.Key.FullName}' interface");

                    _registeredRepositoryTypes.Add(repository.Key, repository.Value);
                }
                else
                    throw new ArgumentException(
                                                $"The type {repository.Value.FullName} doesn't implements 'ILinqRepository<TDataContext>' interface");
            }
        }

        ///<inheritdoc />
        public override void AddRepository(IRepository repository, Type implementedRepository)
        {
            _singletonRepositories[implementedRepository] = repository;
        }
    }
}