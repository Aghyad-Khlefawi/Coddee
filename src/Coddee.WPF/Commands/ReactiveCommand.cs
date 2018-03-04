// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Coddee.Validation;

namespace Coddee.WPF.Commands
{


    /// <summary>
    /// A command that updates the CanExecute property based on properties changes.
    /// </summary>
    /// <typeparam name="TObserved">The observed object(ViewModel) Type</typeparam>
    public abstract class ReactiveCommandBase<TObserved> : IReactiveCommand<TObserved>
    {
        /// <inheritdoc />
        protected ReactiveCommandBase(TObserved observedObject)
        {
            ObservedObject = observedObject;
            _observedProperties = new List<ObservedProperty<TObserved>>();
        }

        /// <summary>
        /// The object being bserved by the command.
        /// </summary>
        public TObserved ObservedObject { get; }

        /// <summary>
        /// The properties that are observed.
        /// </summary>
        protected readonly List<ObservedProperty<TObserved>> _observedProperties;

        /// <summary>
        /// Indicates whether the command can be executed.
        /// </summary>
        protected bool _canExecute = true;

        /// <summary>
        /// Attach an event handler to the <see cref="INotifyPropertyChanged"/>
        /// </summary>
        /// <param name="observedObject"></param>
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




        /// <inheritdoc />
        public IReactiveCommand ObserveProperty(string propertyName, Validator validator)
        {
            _observedProperties.Add(new ObservedProperty<TObserved>(ObservedObject, propertyName, validator));
            UpdateCanExecute();
            return this;
        }
        
        /// <inheritdoc />
        public IReactiveCommand<TObserved> ObserveProperty<TProperty>(string propertyName, Validator<TProperty> validator)
        {
            _observedProperties.Add(new ObservedProperty<TObserved, TProperty>(ObservedObject, propertyName, validator));
            UpdateCanExecute();
            return this;
        }

        /// <inheritdoc />
        public virtual void UpdateCanExecute()

        {
            UISynchronizationContext.ExecuteOnUIContext(() =>
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
            });
        }

        /// <inheritdoc />
        public bool CanExecute()
        {
            return _canExecute;
        }

        /// <inheritdoc />
        public virtual bool CanExecute(object parameter)
        {
            return _canExecute;
        }


        /// <inheritdoc />
        public abstract void Execute(object parameter);


        /// <inheritdoc />
        public event EventHandler CanExecuteChanged;
    }

    /// <summary>
    /// A command that updates the CanExecute property based on properties changes.
    /// </summary>
    /// <typeparam name="TObserved">The observed object(ViewModel) Type</typeparam>
    public class ReactiveCommand<TObserved> : ReactiveCommandBase<TObserved>
    {
        private readonly Action _handler;

        /// <inheritdoc />
        public ReactiveCommand(TObserved observedObject, Action handler)
            : base(observedObject)
        {
            _handler = handler;
        }

        /// <summary>
        /// Create an instance of ReactiveCommand
        /// </summary>
        public static ReactiveCommand<TObserved> Create(TObserved observedObject, Action handler)
        {
            var reactiveCommand = new ReactiveCommand<TObserved>(observedObject, handler);
            if (observedObject is INotifyPropertyChanged notify)
                reactiveCommand.AttachNotifyHandler(notify);
            return reactiveCommand;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public ReactiveCommand(TObserved observedObject, Action<TParam> handler)
            : base(observedObject)
        {
            _handler = handler;
        }

        /// <summary>
        /// Create an instance of ReactiveCommand
        /// </summary>
        public static ReactiveCommand<TObserved, TParam> Create(TObserved observedObject, Action<TParam> handler)
        {
            var reactiveCommand = new ReactiveCommand<TObserved, TParam>(observedObject, handler);
            if (observedObject is INotifyPropertyChanged notify)
                reactiveCommand.AttachNotifyHandler(notify);
            return reactiveCommand;
        }

        /// <inheritdoc />
        public override void Execute(object parameter)
        {
            _handler?.Invoke((TParam)parameter);
        }

    }
}
