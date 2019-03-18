// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;

namespace Coddee.AppBuilder
{
    /// <summary>
    /// Provides the required configurations to use REST repositories.
    /// </summary>
    public class RESTInitializerConfig
    {
        /// <param name="apiUrl">The API base URL</param>
        /// <param name="unauthorizedRequestHandler">An action called when an unauthorized response code received</param>
        /// <param name="repositoriesAssembly">The assembly name containing the repository <remarks>Without extension</remarks></param>
        /// <param name="registerTheRepositoresInContainer">Register the repositories in the dependency container</param>
        public RESTInitializerConfig(string apiUrl, Action unauthorizedRequestHandler, string repositoriesAssembly, bool registerTheRepositoresInContainer = true)
        {
            ApiUrl = apiUrl;
            UnauthorizedRequestHandler = unauthorizedRequestHandler;
            RepositoriesAssembly = repositoriesAssembly;
            RegisterTheRepositoresInContainer = registerTheRepositoresInContainer;
        }

        /// <param name="apiUrl">The API base URL</param>
        /// <param name="unauthorizedRequestHandler">An action called when an unauthorized response code received</param>
        /// <param name="repositoriesTypes">The repositories to register The key is the interface type and the value is the implementation type</param>
        /// <param name="repositoriesAssembly">The assembly name containing the repository <remarks>Without extension</remarks></param>
        /// <param name="registerTheRepositoresInContainer">Register the repositories in the dependency container</param>
        public RESTInitializerConfig(string apiUrl, Action unauthorizedRequestHandler, KeyValuePair<Type, Type>[] repositoriesTypes, string repositoriesAssembly, bool registerTheRepositoresInContainer = true)
            : this(apiUrl, unauthorizedRequestHandler, repositoriesAssembly, registerTheRepositoresInContainer)
        {
            RepositoriesTypes = repositoriesTypes;
        }

        /// <param name="apiUrl">The API base URL</param>
        /// <param name="repositoriesTypes">The repositories to register The key is the interface type and the value is the implementation type</param>
        /// <param name="registerTheRepositoresInContainer">Register the repositories in the dependency container</param>
        public RESTInitializerConfig(string apiUrl, KeyValuePair<Type, Type>[] repositoriesTypes, bool registerTheRepositoresInContainer = true)
            : this(apiUrl, null, repositoriesTypes, null, registerTheRepositoresInContainer)
        {
            RepositoriesTypes = repositoriesTypes;
        }

        /// <summary>
        /// The API base URL
        /// </summary>
        public string ApiUrl { get; set; }

        /// <summary>
        /// An action called when an unauthorized response code received
        /// </summary>
        public Action UnauthorizedRequestHandler { get; set; }

        /// <summary>
        /// The assembly name containing the repository
        /// <remarks>Without extension</remarks>
        /// </summary>
        public string RepositoriesAssembly { get; set; }

        /// <summary>
        /// Register the repositories in the dependency container
        /// </summary>
        public bool RegisterTheRepositoresInContainer { get; set; }


        /// <summary>
        /// The repository types that will be registered
        /// </summary>
        public KeyValuePair<Type, Type>[] RepositoriesTypes { get; }

        /// <summary>
        /// Specifies the timeout for the Http client.
        /// </summary>
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(100);

    }
}