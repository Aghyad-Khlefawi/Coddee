// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;
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
        public ObservedProperty(T obj, Expression<Func<T, object>> observedField, Validator validator)
            : this(obj, ((MemberExpression)observedField.Body).Member.Name, validator)
        {
        }

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


        public string ObservedPropertyName { get; }

        /// <summary>
        /// The property value validator.
        /// </summary>
        public Validator Validator { get; }
    }

    /// <summary>
    /// A command that updates the CanExecute property based on properties changes.
    /// </summary>
    /// <typeparam name="TObserved">The observed object(ViewModel) Type</typeparam>
    public abstract class ReactiveCommandBase<TObserved> : ICommand
    {
        protected ReactiveCommandBase(TObserved observedObject)
        {
            _observedObject = observedObject;
            _observedProperties = new List<ObservedProperty<TObserved>>();
        }

        protected readonly TObserved _observedObject;
        protected readonly List<ObservedProperty<TObserved>> _observedProperties;

        protected bool _canExecute = true;

        protected virtual void AttachNotifyHandler(INotifyPropertyChanged observedObject)
        {
            observedObject.PropertyChanged += (sender, args) =>
            {
                if (_observedProperties.Any(e => e.ObservedPropertyName == args.PropertyName))
                {
                    UpdateCanExecute();
                }
            };
        }

        public virtual void UpdateCanExecute()
        {
            bool canExecute = true;
            foreach (var property in _observedProperties)
            {
                if (!property.Validate())
                {
                    canExecute = false;
                    break;
                }
            }
            if (_canExecute != canExecute)
            {
                _canExecute = canExecute;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public virtual bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public abstract void Execute(object parameter);


        public event EventHandler CanExecuteChanged;
    }

    /// <summary>
    /// A command that updates the CanExecute property based on properties changes.
    /// </summary>
    /// <typeparam name="TObserved">The observed object(ViewModel) Type</typeparam>
    public class ReactiveCommand<TObserved> : ReactiveCommandBase<TObserved>
    {
        private readonly Action _handler;

        public ReactiveCommand(TObserved observedObject, Action handler)
            : base(observedObject)
        {
            _handler = handler;
        }
        public static ReactiveCommand<TObserved> Create(TObserved observedObject, Action handler)
        {
            var reactiveCommand = new ReactiveCommand<TObserved>(observedObject, handler);
            if (observedObject is INotifyPropertyChanged notify)
                reactiveCommand.AttachNotifyHandler(notify);
            return reactiveCommand;
        }

        public ReactiveCommand<TObserved> ObserveProperty(Expression<Func<TObserved, object>> propertyName, Validator validator)
        {
            return ObserveProperty(((MemberExpression)propertyName.Body).Member.Name, validator);
        }
        public ReactiveCommand<TObserved> ObserveProperty(string propertyName, Validator validator)
        {
            _observedProperties.Add(new ObservedProperty<TObserved>(_observedObject, propertyName, validator));
            UpdateCanExecute();
            return this;
        }

        public override void Execute(object parameter)
        {
            _handler?.Invoke();
        }

    }

    /// <summary>
    /// A command that updates the CanExecute property based on properties changes.
    /// </summary>
    /// <typeparam name="TObserved">The observed object(ViewModel) Type</typeparam>
    /// <typeparam name="TParam">The command parameter type</typeparam>
    public class ReactiveCommand<TObserved, TParam> : ReactiveCommandBase<TObserved>
    {
        private readonly Action<TParam> _handler;

        public ReactiveCommand(TObserved observedObject, Action<TParam> handler)
            : base(observedObject)
        {
            _handler = handler;
        }

        public static ReactiveCommand<TObserved, TParam> Create(TObserved observedObject, Action<TParam> handler)
        {
            var reactiveCommand = new ReactiveCommand<TObserved, TParam>(observedObject, handler);
            if (observedObject is INotifyPropertyChanged notify)
                reactiveCommand.AttachNotifyHandler(notify);
            return reactiveCommand;
        }
        public ReactiveCommand<TObserved, TParam> ObserveProperty(Expression<Func<TObserved, object>> propertyName, Validator validator)
        {
            return ObserveProperty(((MemberExpression)propertyName.Body).Member.Name, validator);
        }
        public ReactiveCommand<TObserved, TParam> ObserveProperty(string propertyName, Validator validator)
        {
            _observedProperties.Add(new ObservedProperty<TObserved>(_observedObject, propertyName, validator));
            UpdateCanExecute();
            return this;
        }

        public override void Execute(object parameter)
        {
            _handler?.Invoke((TParam)parameter);
        }

    }
}
