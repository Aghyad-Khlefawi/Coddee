// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Data;

namespace Coddee.Windows.Data.FileRepository
{
    /// <summary>
    /// A <see cref="IRepositoryInitializer"/> for FileRepositories
    /// </summary>
    public class FileRepositoryInitializer : IRepositoryInitializer
    {
        private readonly string _filesLocation;
        private readonly IObjectMapper _mapper;
        private readonly RepositoryConfigurations _config;

        /// <inheritdoc />
        public FileRepositoryInitializer(string filesLocation, IObjectMapper mapper, RepositoryConfigurations config)
        {
            _filesLocation = filesLocation;
            _mapper = mapper;
            _config = config;
        }

        /// <inheritdoc />
        public int RepositoryType { get; } = (int)RepositoryTypes.File;

        /// <inheritdoc />
        public void InitializeRepository(IRepositoryManager repositoryManager, IRepository repository, Type implementedInterface)
        {
            ((IFileRepository)repository).Initialize(_filesLocation, repositoryManager, _mapper, implementedInterface, _config);
        }
    }
}
