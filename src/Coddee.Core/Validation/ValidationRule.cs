// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Linq.Expressions;

namespace Coddee.Validation
{
    /// <summary>
    /// Default implementation for <see cref="IValidationRule"/>
    /// </summary>
    public class ValidationRule : IValidationRule
    {
        /// <summary>
        /// Create a new validation warning rule
        /// </summary>
        /// <param name="validator">The validation function</param>
        /// <param name="message">Warning message in case the validation function returned false.</param>
        public static ValidationRule CreateWarningRule(Validator validator, Func<string> message)
        {
            return new ValidationRule(ValidationType.Warning, validator, message);
        }

        /// <summary>
        /// Create a new validation warning rule
        /// </summary>
        /// <param name="validator">The validation function</param>
        /// <param name="validatedField">The field targeted by the validation rule.</param>
        public static ValidationRule CreateWarningRule<T>(Expression<Func<T>> validatedField, Validator<T> validator)
        {
            return new ValidationRule(ValidationType.Warning, e => validator((T)e), ExpressionHelper.GetMemberName(validatedField), () => validatedField.Compile()());
        }

        /// <summary>
        /// Create a new validation warning rule
        /// </summary>
        /// <param name="validator">The validation function</param>
        /// <param name="validatedField">The field targeted by the validation rule.</param>
        /// <param name="message">Warning message in case the validation function returned false.</param>
        public static ValidationRule CreateWarningRule<T>(Expression<Func<T>> validatedField, Validator<T> validator, Func<string> message)
        {
            return new ValidationRule(ValidationType.Warning, e => validator((T)e), message, ExpressionHelper.GetMemberName(validatedField), () => validatedField.Compile()());
        }

        /// <summary>
        /// Create a new validation warning rule
        /// </summary>
        /// <param name="validatedField">The field targeted by the validation rule.</param>
        /// <param name="message">Warning message in case the validation function returned false.</param>
        public static ValidationRule CreateWarningRule<T>(Expression<Func<T>> validatedField, Func<string> message)
        {
            return new ValidationRule(ValidationType.Warning, e => Validators.GetValidator(typeof(T))(e), message, ExpressionHelper.GetMemberName(validatedField), () => validatedField.Compile()());
        }

        /// <summary>
        /// Create a new validation warning rule
        /// </summary>
        /// <param name="validatedField">The field targeted by the validation rule.</param>
        public static ValidationRule CreateWarningRule<T>(Expression<Func<T>> validatedField)
        {
            return new ValidationRule(ValidationType.Warning, e => Validators.GetValidator(typeof(T))(e), ExpressionHelper.GetMemberName(validatedField), () => validatedField.Compile()());
        }

        /// <summary>
        /// Create a new validation error rule
        /// </summary>
        /// <param name="validator">The validation function</param>
        /// <param name="message">Error message in case the validation function returned false.</param>
        public static ValidationRule CreateErrorRule(Validator validator, Func<string> message)
        {
            return new ValidationRule(ValidationType.Error, validator, message);
        }

        /// <summary>
        /// Create a new validation error rule
        /// </summary>
        /// <param name="validator">The validation function</param>
        /// <param name="validatedField">The field targeted by the validation rule.</param>
        public static ValidationRule CreateErrorRule<T>(Expression<Func<T>> validatedField, Validator<T> validator)
        {
            return new ValidationRule(ValidationType.Error, e => validator((T)e), ExpressionHelper.GetMemberName(validatedField), () => validatedField.Compile()());
        }

        /// <summary>
        /// Create a new validation error rule
        /// </summary>
        /// <param name="validator">The validation function</param>
        /// <param name="validatedField">The field targeted by the validation rule.</param>
        /// <param name="message">Error message in case the validation function returned false.</param>
        public static ValidationRule CreateErrorRule<T>(Expression<Func<T>> validatedField, Validator<T> validator, Func<string> message)
        {
            return new ValidationRule(ValidationType.Error, e => validator((T)e), message, ExpressionHelper.GetMemberName(validatedField), () => validatedField.Compile()());
        }

        /// <summary>
        /// Create a new validation error rule
        /// </summary>
        /// <param name="validatedField">The field targeted by the validation rule.</param>
        /// <param name="message">Error message in case the validation function returned false.</param>
        public static ValidationRule CreateErrorRule<T>(Expression<Func<T>> validatedField, Func<string> message)
        {
            return new ValidationRule(ValidationType.Error, e => Validators.GetValidator(typeof(T))(e), message, ExpressionHelper.GetMemberName(validatedField), () => validatedField.Compile()());
        }

        /// <summary>
        /// Create a new validation error rule
        /// </summary>
        /// <param name="validatedField">The field targeted by the validation rule.</param>
        public static ValidationRule CreateErrorRule<T>(Expression<Func<T>> validatedField)
        {
            return new ValidationRule(ValidationType.Error, e => Validators.GetValidator(typeof(T))(e), ExpressionHelper.GetMemberName(validatedField), () => validatedField.Compile()());
        }

        private ValidationRule(ValidationType validationType, Validator validator, Func<string> message)
        {
            ValidationType = validationType;
            Validator = validator;
            Message = message;
        }

        /// <inheritdoc />
        public ValidationRule(ValidationType validationType, Validator validator, Func<string> message, string fieldName) : this(validationType, validator, message)
        {
            FieldName = fieldName;
        }

        /// <inheritdoc />
        public ValidationRule(ValidationType validationType, Validator validator, Func<string> message, string fieldName, Func<object> validatedField) : this(validationType, validator, message, fieldName)
        {
            ValidatedField = validatedField;
        }
        /// <inheritdoc />
        public ValidationRule(ValidationType validationType, Validator validator, string fieldName, Func<object> validatedField) : this(validationType, validator, () => DefaultMessages.ValidationMessage.Replace("$Field$", fieldName), fieldName)
        {
            ValidatedField = validatedField;
        }

        /// <summary>
        /// The validation result type.
        /// </summary>
        public ValidationType ValidationType { get; }

        /// <summary>
        /// The validation function
        /// </summary>
        public Validator Validator { get; }

        /// <summary>
        /// A function that returns the message that will be displayed 
        /// if the validation fails
        /// </summary>
        public Func<string> Message { get; set; }

        /// <summary>
        /// The valdiated field name.
        /// </summary>
        public string FieldName { get; set; }


        /// <summary>
        /// Test if the field is valid.
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            if (ValidatedField != null)
            {
                var value = ValidatedField();
                return Validator(value);
            }
            return Validator(null);
        }

        /// <summary>
        /// Returns the fail message.
        /// </summary>
        /// <returns></returns>
        public string GetMessage()
        {
            return Message();
        }

        /// <summary>
        /// A function that returns the current value of the validated field.
        /// </summary>
        public Func<object> ValidatedField { get; set; }
    }
}
