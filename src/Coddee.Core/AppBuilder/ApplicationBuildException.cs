// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;

namespace Coddee.AppBuilder
{
    /// <summary>
    /// An exception that occurs in the application startup process.
    /// </summary>
    public class ApplicationBuildException : Exception
    {
        /// <inheritdoc />
        public ApplicationBuildException()
        {
        }

        /// <inheritdoc />
        public ApplicationBuildException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public ApplicationBuildException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
