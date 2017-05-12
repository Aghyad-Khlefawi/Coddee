using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Coddee.Collections;
using Coddee.WPF.Commands;
using Coddee.WPF.Controls;
using Coddee.WPF.Modules;

namespace Coddee.WPF.DebugTool
{
    public class DebugToolViewModel : ViewModelBase<DebugToolView>, IDebugTool
    {
        /// <summary>
        /// The condition to toggle the tool window
        /// </summary>
        private Func<KeyEventArgs, bool> _toggleCondition;
        private bool _windowVisible;

        private AsyncObservableCollection<ViewModelItem> _viewModels;
        public AsyncObservableCollection<ViewModelItem> ViewModels
        {
            get { return _viewModels; }
            set { SetProperty(ref this._viewModels, value); }
        }


        private AsyncObservableCollection<PropertyItem> _properties;
        public AsyncObservableCollection<PropertyItem> Properties
        {
            get { return _properties; }
            set { SetProperty(ref this._properties, value); }
        }
        public ICommand RefreshCommand => new RelayCommand(Refresh);

        private void Refresh()
        {
            GetViewModels();
        }
        protected override Task OnInitialization()
        {
            var shellWindow = (Window) Resolve<IShell>();
            shellWindow.KeyDown += (sender, args) =>
            {
                if (_toggleCondition(args))
                    ToggleWindow();
            };
            return base.OnInitialization();
        }

        private void VMTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> args)
        {
            var item = args.NewValue as ViewModelItem;
            Properties =
                new AsyncObservableCollection<PropertyItem>(item.ViewModel.GetType()
                                                                .GetProperties()
                                                                .Select(e => new PropertyItem(e,
                                                                                              item
                                                                                                  .ViewModel)));
        }

        private void ToggleWindow()
        {
            _windowVisible = !_windowVisible;
            if (_windowVisible)
            {
                GetViewModels();
                CreateView();
                View.VMTree.SelectedItemChanged += VMTree_SelectedItemChanged;
                View.Show();
            }
            else
                View.Close();
        }

        private void GetViewModels()
        {
            var shellViewModel = Resolve<IShellViewModel>();
            ViewModels = new AsyncObservableCollection<ViewModelItem>
            {
                new ViewModelItem(shellViewModel)
            };
        }

        public void SetToggleCondition(Func<KeyEventArgs, bool> toggleCondition)
        {
            _toggleCondition = toggleCondition;
        }
    }

    public class PropertyItem : BindableBase
    {
        public PropertyItem(PropertyInfo property, object item)
        {
            PropertyInfo = property;
            Item = item;

            Name = PropertyInfo.Name;
            var val = PropertyInfo.GetValue(item);
            Value = val?.ToString() ?? "NULL";

        }

        public object Item { get; set; }
        public PropertyInfo PropertyInfo { get; set; }

        private AsyncObservableCollection<PropertyItem> _properties;
        public AsyncObservableCollection<PropertyItem> Properties
        {
            get { return _properties; }
            set { SetProperty(ref this._properties, value); }
        }

        private bool _isPremitive;
        public bool IsPremitive
        {
            get { return _isPremitive; }
            set { SetProperty(ref this._isPremitive, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref this._name, value); }
        }

        private string _value;
        public string Value
        {
            get { return _value; }
            set { SetProperty(ref this._value, value); }
        }
    }

    public class ViewModelItem : BindableBase
    {
        public ViewModelItem(IViewModel viewModel)
        {
            ViewModel = viewModel;
            ViewModels =
                new AsyncObservableCollection<ViewModelItem>(viewModel.ChildViewModels
                                                                 .Select(e => new ViewModelItem(e)));
        }

        public string Name => ViewModel.GetType().Name;
        public IViewModel ViewModel { get; set; }

        private AsyncObservableCollection<ViewModelItem> _viewModels;
        public AsyncObservableCollection<ViewModelItem> ViewModels
        {
            get { return _viewModels; }
            set { SetProperty(ref this._viewModels, value); }
        }
    }
}