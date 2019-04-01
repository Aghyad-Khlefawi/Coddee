// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Net.Http;

namespace Coddee.Data.REST
{
    /// <summary>
    /// Defines the requirements for a repository that uses a REST api
    /// </summary>
    public interface IRESTRepository
    {
        /// <summary>
        /// Initialize the Rest repository
        /// </summary>
        /// <param name="httpClient">The http client.</param>
        /// <param name="addTimeStampToRequests"></param>
        /// <param name="unauthorizedRequestHandler">An action that will be executed in case of an Unauthorized response.</param>
        /// <param name="repositoryManager">The repository manager holding the repository</param>
        /// <param name="mapper">Object mapper</param>
        /// <param name="implementedInterface">The repository interface implemented by this repository</param>
        /// <param name="config">Additional repository configurations.</param>
        void Initialize(HttpClient httpClient,
                        bool addTimeStampToRequests,
                        Action unauthorizedRequestHandler,
                        IRepositoryManager repositoryManager,
                        IObjectMapper mapper,
                        Type implementedInterface,
                        RepositoryConfigurations config = null);

        
        /// <summary>
        /// Change the http client this repository is using.
        /// </summary>
        /// <param name="httpClient">The new http client.</param>
        void SetHttpClient(HttpClient httpClient);
    }
}