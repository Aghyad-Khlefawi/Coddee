// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.AppBuilder
{
    /// <summary>
    /// An abstraction of an application
    /// </summary>
    public interface IApplication
    {
        /// <summary>
        /// GUID to identify the application
        /// </summary>
        Guid ApplicationID { get; }

        /// <summary>
        /// The name of the Application
        /// </summary>
        string ApplicationName { get; }


        /// <summary>
        /// The type of the application
        /// </summary>
        ApplicationTypes ApplicationType { get; }
    }
}