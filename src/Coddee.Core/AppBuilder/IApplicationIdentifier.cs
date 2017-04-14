// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="applicatioName"></param>
        /// <param name="applicationID"></param>
        /// <param name="applicationType"></param>
        /// <returns></returns>
        IApplicationBuilder IdentifyApplication(
            string applicatioName,
            Guid applicationID,
            ApplicationTypes applicationType);
    }
}