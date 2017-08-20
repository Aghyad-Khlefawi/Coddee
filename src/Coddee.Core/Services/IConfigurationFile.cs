// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Services
{
    /// <summary>
    /// A configuration file used by the <see cref="IConfigurationManager"/>.
    /// </summary>
    public interface IConfigurationFile
    {
        /// <summary>
        /// The name of the file.
        /// <remarks>The name is like a key that is used to specify this file, it doesn't refer the name of the physical file.</remarks>
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The file path.
        /// <remarks>This should be the full path to the file and should contain the name and extension of the file.</remarks>
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Read a value from the configuration file.
        /// </summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="key">The object key.</param>
        TValue GetValue<TValue>(string key);

        /// <summary>
        /// Read the values from the file.
        /// </summary>
        void ReadFile();

        /// <summary>
        /// Update an element value.
        /// </summary>
        /// <typeparam name="TValue">The type of the element.</typeparam>
        /// <param name="key">The key of the element.</param>
        /// <param name="value">The new value.</param>
        void SetValue<TValue>(string key, TValue value);

        /// <summary>
        /// Read a value from the configuration file.
        /// </summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="key">The object key.</param>
        /// <param name="value">The returned value</param>
        /// <returns>True if the element exists and false if not.</returns>
        bool TryGetValue<TValue>(string key, out TValue value);
    }
}