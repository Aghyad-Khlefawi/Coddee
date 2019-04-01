﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Coddee.Data.REST
{
    /// <summary>
    /// A repository initializer that should provide the Rest repositories with their dependencies to operate.
    /// </summary>
    public class RESTRepositoryInitializer : IRepositoryInitializer
    {
        private readonly Action _unauthorizedRequestHandler;
        private readonly IObjectMapper _mapper;
        private readonly RepositoryConfigurations _config;
        private HttpClient _client;
        private readonly TimeSpan _requestTimeout;

        /// <inheritdoc />
        public int RepositoryType { get; } = (int)RepositoryTypes.REST;

        /// <summary>
        /// When set to true a time stamp parameter will be added to the query string of every request
        /// </summary>
        public bool AddTimestampToRequests{ get; set; }
        
        /// <inheritdoc />
        public RESTRepositoryInitializer(string apiBaseURL, Action unauthorizedRequestHandler, IObjectMapper mapper,
                                         TimeSpan requestTimeout,
                                         RepositoryConfigurations config = null)
        {
            _unauthorizedRequestHandler = unauthorizedRequestHandler;
            _mapper = mapper;
            _config = config;
            _requestTimeout = requestTimeout;
            _client = new HttpClient { BaseAddress = new Uri(apiBaseURL, UriKind.Absolute),Timeout = requestTimeout};
        }

        /// <inheritdoc />
        public void InitializeRepository(IRepositoryManager repositoryManager, IRepository repository, Type implementedInterface)
        {
            ((IRESTRepository)repository).Initialize(_client, AddTimestampToRequests, _unauthorizedRequestHandler, repositoryManager, _mapper, implementedInterface, _config);
        }

        /// <summary>
        /// Change the API base URL for the repositories.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="repositories"></param>
        public void SetApiBaseUrl(string url, IEnumerable<IRESTRepository> repositories)
        {
            _client = new HttpClient { BaseAddress = new Uri(url, UriKind.Absolute),Timeout =_requestTimeout} ;
            repositories.ForEach(e => e.SetHttpClient(_client));
        }
    }
}
