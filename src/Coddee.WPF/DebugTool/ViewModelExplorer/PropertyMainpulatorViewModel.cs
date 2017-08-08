using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Coddee.Collections;
using Coddee.Services.ViewModelManager;
using Coddee.WPF.Commands;

namespace Coddee.WPF.DebugTool
{
    public class PropertyMainpulatorViewModel : ViewModelBase<PropertyMainpulatorView>
    {
        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            Properties = AsyncObservableCollection<IViewModelProperty>.Create();
        }

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
        public AsyncObservableCollection<IViewModelProperty> Properties
        {
            get { return _properties; }
            set { SetProperty(ref this._properties, value); }
        }
    }

    public interface IViewModelProperty
    {
        bool IsEditable { get; }
        void ReadValue();
        void SetValue();
    }

    public abstract class ViewModelProperty : BindableBase, IViewModelProperty
    {
        protected ViewModelProperty(IViewModel viewModel, PropertyInfo prop)
        {
            ViewModel = viewModel;
            PropertyInfo = prop;
            IsEditable = prop.SetMethod != null;
        }


        public IViewModel ViewModel { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public string Name { get; set; }
        public bool IsEditable { get; protected set; }
        public abstract void ReadValue();
        public abstract void SetValue();
    }
    public abstract class ViewModelProperty<T> : ViewModelProperty
    {
        protected ViewModelProperty(IViewModel viewModel, PropertyInfo prop) : base(viewModel, prop)
        {
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyInfo.Name)
                ReadValue();
        }

        protected bool _reading;

        private T _value;
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

        public override void ReadValue()
        {
            _reading = true;
            Value = (T)PropertyInfo.GetValue(ViewModel);
            _reading = false;
        }
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
    public class StringViewModelProperty : ViewModelProperty<string>
    {
        public StringViewModelProperty(IViewModel viewModel, PropertyInfo prop) : base(viewModel, prop)
        {

        }

    }
    public class BoolViewModelProperty : ViewModelProperty<bool>
    {
        public BoolViewModelProperty(IViewModel viewModel, PropertyInfo prop) : base(viewModel, prop)
        {

        }
    }


    public class ObjectViewModelProperty : ViewModelProperty<string>
    {
        public ObjectViewModelProperty(IViewModel viewModel, PropertyInfo prop) : base(viewModel, prop)
        {

        }

        public override void ReadValue()
        {
            _reading = true;
            Value = PropertyInfo.GetValue(ViewModel)?.GetType().ToString()??"NULL";
            _reading = false;
        }
    }

    public class CommandViewModelProperty : ViewModelProperty<ICommand>
    {
        public CommandViewModelProperty(IViewModel viewModel, PropertyInfo prop) : base(viewModel, prop)
        {

        }

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