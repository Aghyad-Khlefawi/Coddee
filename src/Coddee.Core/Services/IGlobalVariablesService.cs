// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Services
{
    /// <summary>
    /// A service responsible for storing the global variables that are shared across the application.
    /// </summary>
    public interface IGlobalVariablesService
    {
        /// <summary>
        /// Returns the instance of the global variable.
        /// </summary>
        T GetVariable<T>() where T : IGlobaleVariable, new();
    }
}
