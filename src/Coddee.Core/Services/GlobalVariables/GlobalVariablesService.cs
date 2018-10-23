// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;

namespace Coddee.Services
{
    /// <inheritdoc />
    public class GlobalVariablesService : IGlobalVariablesService
    {
        /// <summary>
        /// A dictionary to store the variables values
        /// </summary>
        private readonly Dictionary<Type, IGlobalVariable> _variables;

        /// <inheritdoc />
        public GlobalVariablesService()
        {
            _variables = new Dictionary<Type, IGlobalVariable>();
        }


        /// <inheritdoc />
        public T GetVariable<T>() where T : IGlobalVariable, new()
        {
            if (!_variables.ContainsKey(typeof(T)))
                _variables[typeof(T)] = new T();
            return (T)_variables[typeof(T)];
        }
    }
}
