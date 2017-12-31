// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using Coddee.Validation;

namespace Coddee.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationResult ValidationResult { get; }

        public ValidationException()
        {
        }

        public ValidationException(string message) : base(message)
        {
        }
        public ValidationException(ValidationResult validationResult) : base("Validation returned an invalid result.")
        {
            ValidationResult = validationResult;
        }

        public ValidationException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
