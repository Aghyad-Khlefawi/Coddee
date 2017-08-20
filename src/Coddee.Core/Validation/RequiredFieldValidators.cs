// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

namespace Coddee.Validation
{
    public class RequiredFieldValidators
    {
        public static Validator StringValidator = e =>
        {
            var value = e as string;
            return !string.IsNullOrEmpty(value?.Trim()) && !string.IsNullOrWhiteSpace(value.Trim());
        };

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
    }
}
