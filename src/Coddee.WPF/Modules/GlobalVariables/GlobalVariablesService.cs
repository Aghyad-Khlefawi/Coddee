using System.Collections.Generic;
using Coddee.Services;

namespace Coddee.WPF.Modules
{
    public class GlobalVariablesService: IGlobalVariablesService
    {
        /// <summary>
        /// A dictionary to store the variables values
        /// </summary>
        private readonly Dictionary<string, object> _variables;

        public GlobalVariablesService()
        {
            _variables = new Dictionary<string, object>();
        }

        /// <summary>
        /// Set a value to global variable
        /// </summary>
        public void SetValue(string key, object value)
        {
            if (_variables.ContainsKey(key))
                _variables[key] = value;
            else
                _variables.Add(key, value);
        }
        
        /// <summary>
        /// Get a value of global variable
        /// if the variable is not defined an exception will be thrown
        /// </summary>
        public T GetValue<T>(string key)
        {
            return (T)_variables[key];
        }

        /// <summary>
        /// Get a value of global variable
        /// if the variable is not defined the function will return false
        /// </summary>
        public bool TryGetValue<T>(string key, out T result)
        {
            if (_variables.ContainsKey(key))
            {
                result = (T)_variables[key];
                return true;
            }

            result = default(T);
            return false;
        }

        /// <summary>
        /// Get a value of global variable
        /// if the variable is not defined an exception will be thrown
        /// </summary>
        public object this[string key]
        {
            get
            {
                return !_variables.ContainsKey(key) ? null : _variables[key];
            }
            set { SetValue(key, value); }
        }
        public IDictionary<string, object> GetAllGlobals()
        {
            return _variables;
        }
    }
}
