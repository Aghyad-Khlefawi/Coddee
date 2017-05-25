using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Coddee.WPF.Validation
{
    public class RequiredFieldCollection : List<RequiredField>
    {
    }

    public class RequiredField
    {

        public object Item { get; set; }
        public string FieldName { get; set; }
        public string ErrorMessage { get; set; }
        public Func<object, bool> ValidateField { get; set; }


        public static RequiredField Create<T>(T item,
                                              string fieldName,
                                              Func<object, bool> validator,
                                              string errorMessage)
        {
            return new RequiredField
            {
                Item = item,
                FieldName = fieldName,
                ValidateField = validator,
                ErrorMessage = errorMessage
            };
        }
        public static RequiredField Create<T>(T item,
                                              string fieldName,
                                              Func<object, bool> validator)
        {
           return Create(item, fieldName, validator, LocalizationManager.DefaultLocalizationManager.GetValue("DefaultValidationMessage").Replace("$FieldName$", fieldName));
        }

        public static RequiredField Create<T>(T item,
                                              Expression<Func<T, object>> field,
                                              Func<object, bool> validator,
                                              string errorMessage)
        {
            var fieldName = ((MemberExpression) field.Body).Member.Name;
            return Create(item, fieldName, validator, errorMessage);
        }

        public static RequiredField Create<T>(T item,
                                              Expression<Func<T, object>> field,
                                              Func<object, bool> validator)
        {
            var fieldName = ((MemberExpression)field.Body).Member.Name;
            return Create(item, fieldName, validator, LocalizationManager.DefaultLocalizationManager.GetValue("DefaultValidationMessage").Replace("$FieldName$", fieldName));
        }

        public static RequiredField Create(string fieldName,
                                              Func<object, bool> validator,
                                              string errorMessage)
        {
            return new RequiredField
            {
                FieldName = fieldName,
                ValidateField = validator,
                ErrorMessage = errorMessage
            };
        }
        public static RequiredField Create(string fieldName,
                                              Func<object, bool> validator)
        {
            return Create(fieldName, validator, LocalizationManager.DefaultLocalizationManager.GetValue("DefaultValidationMessage").Replace("$FieldName$", fieldName));
        }
        }
}