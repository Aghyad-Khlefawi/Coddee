// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Coddee.Collections;
using Coddee.Mvvm;
using Coddee.WPF.Commands;

namespace Coddee.WPF.DebugTool
{
    /// <summary>
    /// Provides the ability to change ViewModels properties values at runtime.
    /// </summary>
    public class PropertyMainpulatorViewModel : ViewModelBase<PropertyMainpulatorView>
    {
        /// <inheritdoc />
        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            Properties = AsyncObservableCollection<IViewModelProperty>.Create();
        }

        /// <summary>
        /// Set the targeted ViewModel
        /// </summary>
        public void SetViewModel(ViewModelInfo vm)
        {
            var type = vm.ViewModel.GetType();
            Properties.Clear();
            foreach (var prop in type.GetProperties().OrderBy(e => e.Name))
            {
                IViewModelProperty property = null;
                if (prop.PropertyType == typeof(string))
                {
                    property = new StringViewModelProperty(vm.ViewModel, prop);
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    property = new BoolViewModelProperty(vm.ViewModel, prop);
                }
                else if (prop.PropertyType == typeof(ICommand) || prop.PropertyType.GetInterfaces().Any(e => e == typeof(ICommand)))
                {
                    property = new CommandViewModelProperty(vm.ViewModel, prop);
                }

                if (property == null)
                    property = new ObjectViewModelProperty(vm.ViewModel, prop);
                property.ReadValue();
                Properties.Add(property);

            }
        }

        private AsyncObservableCollection<IViewModelProperty> _properties;
        
        /// <summary>
        /// The properties available in the ViewModel.
        /// </summary>
        public AsyncObservableCollection<IViewModelProperty> Properties
        {
            get { return _properties; }
            set { SetProperty(ref this._properties, value); }
        }
    }

    /// <summary>
    /// Wraps a Property in a ViewModel.
    /// </summary>
    public interface IViewModelProperty
    {
        /// <summary>
        /// Indicates whether the property value can be changed.
        /// </summary>
        bool IsEditable { get; }

        /// <summary>
        /// Read the property value.
        /// </summary>
        void ReadValue();
        
        /// <summary>
        /// Set the property value.
        /// </summary>
        void SetValue();
    }

    /// <summary>
    /// Wraps a Property in a ViewModel.
    /// </summary>
    public abstract class ViewModelProperty : BindableBase, IViewModelProperty
    {
        /// <inheritdoc />
        protected ViewModelProperty(IViewModel viewModel, PropertyInfo prop)
        {
            ViewModel = viewModel;
            PropertyInfo = prop;
            IsEditable = prop.SetMethod != null;
        }

        /// <summary>
        /// The ViewModel instance that contains the property.
        /// </summary>
        public IViewModel ViewModel { get; set; }

        /// <summary>
        /// The property information object. 
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// The name of the property.
        /// </summary>
        public string Name { get; set; }
        
        /// <inheritdoc />
        public bool IsEditable { get; protected set; }
        /// <inheritdoc />
        public abstract void ReadValue();
        /// <inheritdoc />
        public abstract void SetValue();
    }

    /// <summary>
    /// Wraps a Property in a ViewModel.
    /// </summary>
    public abstract class ViewModelProperty<T> : ViewModelProperty
    {
        /// <inheritdoc />
        protected ViewModelProperty(IViewModel viewModel, PropertyInfo prop) : base(viewModel, prop)
        {
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyInfo.Name)
                ReadValue();
        }

        /// <summary>
        /// If true then the property value is being read.
        /// </summary>
        protected bool _reading;

        private T _value;

        /// <summary>
        /// The value of the property.
        /// </summary>
        public T Value
        {
            get { return _value; }
            set
            {
                SetProperty(ref this._value, value);
                if (!_reading)
                    SetValue();
            }
        }

        /// <inheritdoc />
        public override void ReadValue()
        {
            _reading = true;
            Value = (T)PropertyInfo.GetValue(ViewModel);
            _reading = false;
        }

        /// <inheritdoc />
        public override void SetValue()
        {
            if (!IsEditable)
                return;
            try
            {
                PropertyInfo.SetValue(ViewModel, Value);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Invalid value.\nException:{e.Message}");
            }
        }

    }

    /// <summary>
    /// Wraps a <see cref="string"/> Property in a ViewModel.
    /// </summary>
    public class StringViewModelProperty : ViewModelProperty<string>
    {
        /// <inheritdoc />
        public StringViewModelProperty(IViewModel viewModel, PropertyInfo prop) : base(viewModel, prop)
        {

        }
    }

    /// <summary>
    /// Wraps a <see cref="bool"/> Property in a ViewModel.
    /// </summary>
    public class BoolViewModelProperty : ViewModelProperty<bool>
    {
        /// <inheritdoc />
        public BoolViewModelProperty(IViewModel viewModel, PropertyInfo prop) : base(viewModel, prop)
        {

        }
    }

    /// <summary>
    /// Wraps an <see cref="object"/> Property in a ViewModel.
    /// </summary>
    public class ObjectViewModelProperty : ViewModelProperty<string>
    {
        /// <inheritdoc />
        public ObjectViewModelProperty(IViewModel viewModel, PropertyInfo prop) : base(viewModel, prop)
        {

        }

        /// <inheritdoc />
        public override void ReadValue()
        {
            _reading = true;
            Value = PropertyInfo.GetValue(ViewModel)?.GetType().ToString() ?? "NULL";
            _reading = false;
        }
    }

    /// <summary>
    /// Wraps an <see cref="ICommand"/> Property in a ViewModel.
    /// </summary>
    public class CommandViewModelProperty : ViewModelProperty<ICommand>
    {
        /// <inheritdoc />
        public CommandViewModelProperty(IViewModel viewModel, PropertyInfo prop) : base(viewModel, prop)
        {

        }

        /// <summary>
        /// Execute the command in the ViewModel.
        /// </summary>
        public ICommand TriggerCommand => new RelayCommand(Trigger);

        private void Trigger()
        {
            try
            {
                Value.Execute(null);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }


    class ViewModelMock : ViewModelBase
    {
        public ViewModelMock(string name)
        {
            __Name = name;

        }

        public RelayCommand TestCommand => new RelayCommand(Command);

        private void Command()
        {

        }
        public bool IsSelected { get; set; }
    }

}