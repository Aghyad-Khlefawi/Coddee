// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.WPF.AppBuilder
{
    /// <summary>
    /// Describe the required functionality to start a WPF applications
    /// </summary>
    public interface IWPFApplicationFactory
    {
        /// <summary>
        /// Create the Application builder
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="applicationID"></param>
        /// <returns></returns>
        IWPFApplicationBuilder CreateWPFApplication(string applicationName, Guid applicationID);
    }
}
