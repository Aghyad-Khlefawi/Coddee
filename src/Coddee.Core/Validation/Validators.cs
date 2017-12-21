// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Linq;

namespace Coddee.Validation
{
    public class Validators
    {
        public static Validator StringValidator = e =>
        {
            var value = e as string;
            return !string.IsNullOrEmpty(value?.Trim()) && !string.IsNullOrWhiteSpace(value.Trim());
        };

        public static Validator StringLengthValidator(int min, int max)
        {
            return e =>
            {
                var value = e as string ?? String.Empty;
                return value.Length >= min && value.Length <= max;
            };
        }

        public static Validator CombinedValidator(params Validator[] validators)
        {
            return obj => validators.All(e => e(obj));
        }

        public static Validator NullableValidator = e => e != null;

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

        public static Validator GetValidator(Type type)
        {
            if (type == typeof(string))
                return StringValidator;

            return NullableValidator;
        }
    }
}
