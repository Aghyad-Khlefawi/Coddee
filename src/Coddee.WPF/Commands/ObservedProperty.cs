// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Linq.Expressions;
using System.Reflection;
using Coddee.Validation;

namespace Coddee.WPF.Commands
{
    /// <summary>
    /// A property information that will cause the ReactiveCommand to update the CanExecute.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservedProperty<T>
    {
        private readonly PropertyInfo _property;

        /// <inheritdoc />
        public ObservedProperty(T obj, Expression<Func<T, object>> observedField, Validator validator)
            : this(obj, ExpressionHelper.GetMemberName(observedField), validator)
        {
        }

        /// <inheritdoc />
        public ObservedProperty(T obj, string observedPropertyName, Validator validator)
        {
            Object = obj;
            ObservedPropertyName = observedPropertyName;
            Validator = validator;
            _property = obj.GetType().GetProperty(observedPropertyName);
        }

        /// <summary>
        /// Validates the property value
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            return Validator?.Invoke(_property.GetValue(Object)) ?? true;
        }

        /// <summary>
        /// The observed object
        /// </summary>
        public T Object { get; }

        /// <summary>
        /// The name of the property beeing observed.
        /// </summary>
        public string ObservedPropertyName { get; }

        /// <summary>
        /// The property value validator.
        /// </summary>
        public Validator Validator { get; }
    }

    /// <summary>
    /// A property information that will cause the ReactiveCommand to update the CanExecute.
    /// </summary>
    public class ObservedProperty<T, TProperty> : ObservedProperty<T>
    {
        /// <inheritdoc />
        public ObservedProperty(T obj, Expression<Func<T, object>> observedField, Validator<TProperty> validator) : base(obj, observedField, e => validator((TProperty)e))
        {
        }

        /// <inheritdoc />
        public ObservedProperty(T obj, string observedPropertyName, Validator<TProperty> validator) : base(obj, observedPropertyName, e => validator((TProperty)e))
        {
        }
    }


}