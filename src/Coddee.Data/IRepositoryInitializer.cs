// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Data
{
    /// <summary>
    /// A repository initializer that should provide the repositories with their dependencies to operate.
    /// </summary>
    public interface IRepositoryInitializer
    {
        /// <summary>
        /// The type of repositories that this initializer can handle. <seealso cref="RepositoryTypes"/>
        /// </summary>
        int RepositoryType { get; }

        /// <summary>
        /// Initialize a repository.
        /// </summary>
        /// <param name="repositoryManager">The repository manager using this initializer.</param>
        /// <param name="repository">The repository to initialize.</param>
        /// <param name="implementedInterface">The IRepository interface that the repository implements.</param>
        void InitializeRepository(IRepositoryManager repositoryManager, IRepository repository, Type implementedInterface);
    }
}
