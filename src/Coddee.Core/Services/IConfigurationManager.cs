// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Services
{
    /// <summary>
    /// service for storing application configurations in one or multiple files
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Initialize the configuration manager.
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <param name="defaultConfigurationFile">The default configuration file to be used if no file name is specified.</param>
        void Initialize(string fileLocation, IConfigurationFile defaultConfigurationFile);

        /// <summary>
        /// Add a new configuration file.
        /// </summary>
        void AddConfigurationFile(IConfigurationFile configFile);

        /// <summary>
        /// Read a value from the configuration file.
        /// </summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="key">The object key.</param>
        /// <param name="fileName">The file from which to get the value. <remarks>if left null then the default file will be used</remarks></param>
        TValue GetValue<TValue>(string key, string fileName = null);

        /// <summary>
        /// Read a value from the configuration file.
        /// </summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="key">The object key.</param>
        /// <param name="value">The returned value</param>
        /// <returns>True if the element exists and false if not.</returns>
        /// <param name="fileName">The file from which to get the value. <remarks>if left null then the default file will be used</remarks></param>
        bool TryGetValue<TValue>(string key, out TValue value, string fileName = null);

        /// <summary>
        /// Update an element value.
        /// </summary>
        /// <typeparam name="TValue">The type of the element.</typeparam>
        /// <param name="key">The key of the element.</param>
        /// <param name="value">The new value.</param>
        /// <param name="fileName">The file from which to get the value. <remarks>if left null then the default file will be used</remarks></param>
        void SetValue<TValue>(string key, TValue value, string fileName = null);
    }
}