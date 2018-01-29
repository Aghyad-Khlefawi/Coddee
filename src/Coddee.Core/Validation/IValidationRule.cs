// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Validation
{
    public enum ValidationType
    {
        Error,
        Warning
    }

    /// <summary>
    /// A component validation rule
    /// </summary>
    public interface IValidationRule
    {
        /// <summary>
        /// The type of the rule (Error, warning ...)
        /// </summary>
        ValidationType ValidationType { get; }

        /// <summary>
        /// The validation function
        /// </summary>
        Validator Validator { get; }

        /// <summary>
        /// The name of the field that is being validated
        /// </summary>
        string FieldName { get; set; }

        /// <summary>
        /// Calls the <see cref="Validator"/> function.
        /// </summary>
        /// <returns></returns>
        bool Validate();

        /// <summary>
        /// Returns the error message in case the validation failed
        /// </summary>
        string GetMessage();
    }
}
