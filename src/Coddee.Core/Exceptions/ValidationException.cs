// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Validation;

namespace Coddee.Exceptions
{
    /// <summary>
    /// An exception that indicates that a validation operation failed.
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// The faild validation result
        /// </summary>
        public ValidationResult ValidationResult { get; }

        /// <inheritdoc />
        public ValidationException()
        {
        }

        /// <inheritdoc />
        public ValidationException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public ValidationException(ValidationResult validationResult) : base("Validation returned an invalid result.")
        {
            ValidationResult = validationResult;
        }

        /// <inheritdoc />
        public ValidationException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
