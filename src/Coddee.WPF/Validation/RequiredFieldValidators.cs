using System;

namespace Coddee.WPF.Validation
{
    public class RequiredFieldValidators
    {
        public static Func<object, bool> StringValidator = e =>
        {
            var value = e as string;
            return value!=null && !string.IsNullOrEmpty(value.Trim()) && !string.IsNullOrWhiteSpace(value.Trim());
        };

        public static Func<object, bool> NullableValidator = e => e != null;
    }
}
