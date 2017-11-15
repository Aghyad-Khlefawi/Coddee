// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Coddee.Services;

namespace Coddee.Validation
{
    public class RequiredFieldCollection : List<RequiredField>
    {
    }

    public class RequiredField
    {
        public object Item { get; set; }
        public string FieldName { get; set; }
        public Func<string> ErrorMessage { get; set; }
        public Validator ValidateField { get; set; }

        public static RequiredField Create<T>(T item,
                                              string fieldName,
                                              Validator validator,
                                              string errorMessage)
        {
            return Create(item,
                          fieldName,
                          validator,
                          () => errorMessage);
        }

        public static RequiredField Create<T>(T item,
                                              string fieldName,
                                              Validator validator,
                                              Func<string> errorMessage)
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
                          LocalizationManager
                              .DefaultLocalizationManager
                              .GetValue("DefaultValidationMessage")
                              .Replace("$FieldName$",
                                       LocalizationManager
                                           .DefaultLocalizationManager
                                           .GetValue(fieldName)));
        }

        public static RequiredField Create<T>(T item,
                                              Expression<Func<T, object>> field,
                                              Validator validator,
                                              string errorMessage)
        {
            return Create(item,
                          field,
                          validator,
                          () => errorMessage);
        }

        public static RequiredField Create<T>(T item,
                                              Expression<Func<T, object>> field,
                                              Validator validator,
                                              Func<string> errorMessage)
        {
            var fieldName = ExpressionHelper.GetMemberName(field);
            return Create(item,
                          fieldName,
                          validator,
                          errorMessage);
        }

        public static RequiredField Create<T>(T item,
                                              Expression<Func<T, object>> field,
                                              Validator validator)
        {
            var fieldName = ExpressionHelper.GetMemberName(field);
            return Create(item,
                          fieldName,
                          validator);
        }

        public static RequiredField Create<T>(T item,
                                              Expression<Func<T, object>> field)
        {
            var fieldName = ExpressionHelper.GetMemberName(field);
            var type = ExpressionHelper.GetMemberType(field);
            return Create(item,
                          fieldName,
                          RequiredFieldValidators
                              .GetValidator(type));
        }

        public static RequiredField Create<T>(T item,
                                              Expression<Func<T, object>> field,
                                              string errorMessage)
        {
            return Create(item,
                          field,
                          () => errorMessage);
        }

        public static RequiredField Create<T>(T item,
                                              Expression<Func<T, object>> field,
                                              Func<string> errorMessage)
        {
            var fieldName = ExpressionHelper.GetMemberName(field);
            var type = ExpressionHelper.GetMemberType(field);
            return Create(item,
                          fieldName,
                          RequiredFieldValidators
                              .GetValidator(type),
                          errorMessage);
        }

        public static RequiredField Create(string fieldName,
                                           Validator validator,
                                           string errorMessage)
        {
            return Create(fieldName,
                          validator,
                          () => errorMessage);
        }

        public static RequiredField Create(string fieldName,
                                           Validator validator,
                                           Func<string> errorMessage)
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
                          LocalizationManager
                              .DefaultLocalizationManager
                              .GetValue("DefaultValidationMessage")
                              .Replace("$FieldName$",
                                       LocalizationManager
                                           .DefaultLocalizationManager
                                           .GetValue(fieldName)));
        }
    }
}