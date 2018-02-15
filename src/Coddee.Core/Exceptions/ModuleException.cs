// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.Services
{
    /// <summary>
    /// An exception that occurs in the modules registration process.
    /// </summary>
    public class ModuleException : Exception
    {
        /// <inheritdoc />
        public ModuleException()
        {
        }

        /// <inheritdoc />
        public ModuleException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public ModuleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}