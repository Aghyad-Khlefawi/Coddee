// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  



namespace Coddee.AppBuilder
{
    /// <summary>
    /// Responsible for invoking the application build steps in the correct order. 
    /// </summary>
    public interface IApplicationBuilder
    {
        /// <summary>
        /// A build action coordinator
        /// </summary>
        BuildActionsCoordinator BuildActionsCoordinator { get; }
    }
}
