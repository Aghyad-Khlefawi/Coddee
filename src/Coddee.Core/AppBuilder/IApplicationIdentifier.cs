// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.AppBuilder
{
    /// <summary>
    /// The first step of bootstrapping the application
    /// </summary>
    public interface IApplicationIdentifier
    {
        /// <summary>
        /// This method define the application name, id and type then returns 
        /// the appropriate application builder
        /// </summary>
        /// <param name="applicatioName">The name of the application.</param>
        /// <param name="applicationID">An Identifier for the application.</param>
        /// <param name="applicationType">The type of the application.</param>
        /// <returns></returns>
        IApplicationBuilder IdentifyApplication(
            string applicatioName,
            Guid applicationID,
            ApplicationTypes applicationType);
    }
}