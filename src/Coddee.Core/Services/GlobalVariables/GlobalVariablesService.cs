using System;
using System.Collections.Generic;

namespace Coddee.Services
{
    public class GlobalVariablesService : IGlobalVariablesService
    {
        /// <summary>
        /// A dictionary to store the variables values
        /// </summary>
        private readonly Dictionary<Type, IGlobaleVariable> _variables;

        public GlobalVariablesService()
        {
            _variables = new Dictionary<Type, IGlobaleVariable>();
        }


        public T GetVariable<T>() where T : IGlobaleVariable, new()
        {
            if (!_variables.ContainsKey(typeof(T)))
                _variables[typeof(T)] = new T();
            return (T)_variables[typeof(T)];
        }
    }
}
