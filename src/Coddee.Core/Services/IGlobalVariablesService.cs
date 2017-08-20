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
        /// <summary>
        /// Triggered when a variable values is changed.
        /// </summary>
        event EventHandler<ValueChangedEventArgs> VariableValueChanged;

        /// <summary>
        /// Set a new value for a variable.
        /// </summary>
        /// <param name="key">The variable's key</param>
        /// <param name="value">The new value</param>
        void SetValue(string key, object value);

        /// <summary>
        /// Return the value of a variable.
        /// </summary>
        /// <typeparam name="T">The type of the variable.</typeparam>
        /// <param name="key">The variable's key</param>
        T GetValue<T>(string key);


        /// <summary>
        /// Return the value of a variable.
        /// </summary>
        /// <typeparam name="T">The type of the variable.</typeparam>
        /// <param name="key">The variable's key</param>
        /// <param name="result">The variable value.</param>
        /// <returns>True if the variable exists and false if not.</returns>
        bool TryGetValue<T>(string key, out T result);

        /// <summary>
        /// Return the value of a variable.
        /// </summary>
        /// <param name="index">The variable's key</param>
        object this[string index] { get; set; }

        /// <summary>
        /// Returns all the available variables.
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> GetAllGlobals();
    }
}
