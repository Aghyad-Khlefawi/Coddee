// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Microsoft.Practices.Unity;

namespace Coddee.AppBuilder
{
    public interface IApplicationBuilder
    {

        /// <summary>
        /// Returns the dependency injection that is being used for the application
        /// </summary>
        /// <returns></returns>
        IUnityContainer GetContainer();
        
        /// <summary>
        /// Set a build step action
        /// </summary>
        void SetBuildAction(string actionName, Action action);
        
        /// <summary>
        /// The last step of building the application
        /// </summary>
        void Start();
    }
}
