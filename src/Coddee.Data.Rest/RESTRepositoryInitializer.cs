// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Coddee.Data.REST
{
    public class RESTRepositoryInitializer : IRepositoryInitializer
    {
        private readonly Action _unauthorizedRequestHandler;
        private readonly IObjectMapper _mapper;
        private HttpClient _client;

        public int RepositoryType { get; } = (int)RepositoryTypes.REST;

        public RESTRepositoryInitializer(string apiBaseURL, Action unauthorizedRequestHandler, IObjectMapper mapper)
        {
            _unauthorizedRequestHandler = unauthorizedRequestHandler;
            _mapper = mapper;
            _client = new HttpClient { BaseAddress = new Uri(apiBaseURL, UriKind.Absolute) };
        }

        public void InitializeRepository(IRepositoryManager repositoryManager, IRepository repository, Type implementedInterface)
        {
            ((IRESTRepository)repository).Initialize(_client, _unauthorizedRequestHandler, repositoryManager, _mapper, implementedInterface);
        }

        public void SetApiBaseUrl(string url, IEnumerable<IRESTRepository> repositories)
        {
            _client = new HttpClient { BaseAddress = new Uri(url, UriKind.Absolute) };
            repositories.ForEach(e => e.SetHttpClient(_client));
        }
    }
}
