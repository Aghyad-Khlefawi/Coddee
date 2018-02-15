// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Linq;

namespace Coddee.Validation
{
    /// <summary>
    /// Static class containing the common validators.
    /// </summary>
    public static class Validators
    {
        /// <summary>
        /// A string validator that checks for Null and white space
        /// </summary>
        public static Validator StringValidator = e =>
        {
            var value = e as string;
            return !string.IsNullOrEmpty(value?.Trim()) && !string.IsNullOrWhiteSpace(value.Trim());
        };

        /// <summary>
        /// A string validator that checks the length of the string.
        /// </summary>
        /// <param name="min">Minimum allowed length</param>
        /// <param name="max">Maximum allowed length</param>
        /// <returns></returns>
        public static Validator StringLengthValidator(int min, int max)
        {
            return e =>
            {
                var value = e as string ?? String.Empty;
                return value.Length >= min && value.Length <= max;
            };
        }

        /// <summary>
        /// Combines two or more validators
        /// </summary>
        public static Validator CombinedValidator(params Validator[] validators)
        {
            return obj => validators.All(e => e(obj));
        }


        /// <summary>
        /// Checks if the validated object is null
        /// </summary>
        public static Validator NullableValidator = e => !object.ReferenceEquals(e, null);

        /// <summary>
        /// Get validator by name
        /// </summary>
        public static Validator GetValidator(string name)
        {
            switch (name)
            {
                case nameof(StringValidator):
                    return StringValidator;

                case nameof(NullableValidator):
                    return NullableValidator;
            }
            return null;
        }

        /// <summary>
        /// Get default validator by type
        /// </summary>
        public static Validator GetValidator(Type type)
        {
            if (type == typeof(string))
                return StringValidator;

            return NullableValidator;
        }
    }
}
