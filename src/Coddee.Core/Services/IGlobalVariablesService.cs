// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;

namespace Coddee.Services
{
    /// <summary>
    /// A service responsible for storing the global variables that are shared across the application.
    /// </summary>
    public interface IGlobalVariablesService
    {

        T GetVariable<T>() where T : IGlobaleVariable, new();
    }
}
