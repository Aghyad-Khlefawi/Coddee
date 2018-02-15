// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  


using System;

namespace Coddee.Services.Configuration
{
    /// <summary>
    /// An exception that occurs while reading or writing the application configurations.
    /// </summary>
    public class ConfigurationException : Exception
    {
        /// <inheritdoc />
        public ConfigurationException()
        {
        }

        /// <inheritdoc />
        public ConfigurationException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public ConfigurationException(string message, Exception inner) : base(message, inner)
        {
        }
        
    }
}
