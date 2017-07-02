using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Coddee.Validation
{
    public class RequiredFieldCollection : List<RequiredField>
    {
    }

    public class RequiredField
    {
        public object Item { get; set; }
        public string FieldName { get; set; }
        public string ErrorMessage { get; set; }
        public Validator ValidateField { get; set; }


        public static RequiredField Create<T>(T item,
                                              string fieldName,
                                              Validator validator,
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
                                              Validator validator)
        {
            return Create(item,
                          fieldName,
                          validator,
                          LocalizationManager.DefaultLocalizationManager.GetValue("DefaultValidationMessage")
                              .Replace("$FieldName$", LocalizationManager.DefaultLocalizationManager.GetValue(fieldName)));
        }

        public static RequiredField Create<T>(T item,
                                              Expression<Func<T, object>> field,
                                              Validator validator,
                                              string errorMessage)
        {
            var fieldName = ((MemberExpression) field.Body).Member.Name;
            return Create(item, fieldName, validator, errorMessage);
        }

        public static RequiredField Create<T>(T item,
                                              Expression<Func<T, object>> field,
                                              Validator validator)
        {
            var fieldName = ((MemberExpression) field.Body).Member.Name;
            return Create(item,
                          fieldName,
                          validator);
        }

        public static RequiredField Create(string fieldName,
                                           Validator validator,
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
                                           Validator validator)
        {
            return Create(fieldName,
                          validator,
                          LocalizationManager.DefaultLocalizationManager.GetValue("DefaultValidationMessage")
                              .Replace("$FieldName$", LocalizationManager.DefaultLocalizationManager.GetValue(fieldName)));
        }
    }
}