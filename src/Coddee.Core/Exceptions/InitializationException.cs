// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Exceptions
{
    /// <summary>
    /// An exception that occurs during the initialization operation of an object.
    /// </summary>
    public class InitializationException : Exception
    {
        /// <inheritdoc />
        public InitializationException()
        {
        }

        /// <inheritdoc />
        public InitializationException(string componentName) : base($"{componentName} Failed to initialize.")
        {
        }

        /// <inheritdoc />
        public InitializationException(string componentName, Exception inner) : base(componentName, inner)
        {
        }
    }
}
