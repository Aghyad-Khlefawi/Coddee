﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Coddee.Data;
using Coddee.Loggers;
using Coddee.Services;
using Coddee.WPF.Modules;
using Coddee.Validation;
using Microsoft.Practices.Unity;

namespace Coddee.WPF
{
    /// <summary>
    /// The base class for all ViewModels of the application
    /// Contains the property changed handlers and UI execute method
    /// </summary>
    public class ViewModelBase : BindableBase, IViewModel, IDataErrorInfo
    {
        private const string _eventsSource = "VMBase";


        protected static readonly Task completedTask = Task.FromResult(false);
        protected static WPFApplication _app;
        protected static IUnityContainer _container;
        protected static IGlobalVariablesService _globalVariables;
        protected static IToastService _toast;
        protected static ILocalizationManager _localization;
        protected static ILogger _logger;

        protected virtual string _vmName { get; set; }
        public string vmName
        {
            get { return _vmName ?? GetType().Name; }
            set { _vmName = value; }
        }


        public ViewModelBase()
        {
            Initialized += OnInitialized;
            ChildCreated += OnChildCreated;
            ChildViewModels = new List<IViewModel>();
            RequiredFields = new RequiredFieldCollection();
            _requiredFieldsPropertyInfo = new Dictionary<string, PropertyInfo>();

            if (IsDesignMode())
                OnDesignMode();
        }

        public RequiredFieldCollection RequiredFields { get; }

        protected readonly Dictionary<string, PropertyInfo> _requiredFieldsPropertyInfo;

        public event EventHandler Initialized;
        public event EventHandler<IViewModel> ChildCreated;

        public IViewModel ParentViewModel { get; set; }

        public IList<IViewModel> ChildViewModels { get; protected set; }

        private bool _isInitialized;
        public bool IsInitialized
        {
            get { return _isInitialized; }
            protected set { SetProperty(ref this._isInitialized, value); }
        }

        private bool _isBusy = true;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref this._isBusy, value); }
        }

        protected async Task<IViewModel> InitializeViewModel(Type viewModelType)
        {
            var vm = CreateViewModel(viewModelType);
            await vm.Initialize();
            return vm;
        }

        protected virtual void OnDesignMode()
        {
            IsBusy = false;
        }

        protected async Task<TResult> InitializeViewModel<TResult>() where TResult : IViewModel
        {
            return (TResult) await InitializeViewModel(typeof(TResult));
        }

        public IViewModel CreateViewModel(Type viewModelType)
        {
            IViewModel vm = (IViewModel) Resolve(viewModelType);
            _logger?.Log(_eventsSource, $"ViewModelCreated {vmName}", LogRecordTypes.Debug);
            AddChildViewModel(vm);
            return vm;
        }

        protected virtual Task IntitilzieChildViewModels()
        {
            return Task.WhenAll(ChildViewModels.Select(e => e.Initialize()));
        }

        protected virtual void AddChildViewModel(IViewModel vm)
        {
            if (!ChildViewModels.Contains(vm))
            {
                vm.ParentViewModel = this;
                ChildViewModels.Add(vm);
                ChildCreated?.Invoke(this, vm);
                vm.ChildCreated += ChildCreated;
            }
        }

        protected virtual void AddChildViewModels(params object[] vms)
        {
            foreach (var obj in vms)
            {
                var vm = obj as IViewModel;
                if (vm != null)
                    AddChildViewModel(vm);
            }
        }

        public TResult CreateViewModel<TResult>() where TResult : IViewModel
        {
            return (TResult) CreateViewModel(typeof(TResult));
        }

        protected virtual void OnChildCreated(object sender, IViewModel e)
        {
            _logger?.Log(_eventsSource, $"Child view model added {vmName}", LogRecordTypes.Debug);
        }

        /// <summary>
        /// Return the running application instance
        /// </summary>
        /// <param name="app"></param>
        public static void SetApplication(WPFApplication app)
        {
            _app = app;
        }

        /// <summary>
        /// Returns the application Dispatcher
        /// </summary>
        protected Dispatcher GetDispatcher()
        {
            return IsDesignMode() ? Application.Current.Dispatcher : _app.GetSystemApplication().Dispatcher;
        }

        /// <summary>
        /// Executes and action on the UI SynchronizationContext
        /// </summary>
        /// <param name="action"></param>
        protected void ExecuteOnUIContext(Action action)
        {
            UISynchronizationContext.ExecuteOnUIContext(action);
        }

        protected virtual Task OnInitialization()
        {
            return completedTask;
        }

        /// <summary>
        /// Called when the ViewModel is ready to be presented
        /// </summary>
        /// <returns></returns>
        public Task Initialize()
        {
            _logger?.Log(_eventsSource, $"ViewModel initializing {vmName}", LogRecordTypes.Debug);
            return Task.Run(() =>
            {
                OnInitialization().Wait();

                RefreshRequiredFields();

                IsInitialized = true;
                IsBusy = false;
                Initialized?.Invoke(this, EventArgs.Empty);
            });
        }

        protected virtual void RefreshRequiredFields()
        {
            RequiredFields.Clear();
            SetRequiredFields(RequiredFields);
            GetRequiredPropertiesInfo();
        }

        protected virtual void OnInitialized(object sender, EventArgs e)
        {
            _logger?.Log(_eventsSource, $"ViewModel initialized {vmName}", LogRecordTypes.Debug);
        }

        /// <summary>
        /// Set the container on the view model and resolve the basic dependencies
        /// </summary>
        /// <param name="container"></param>
        public static void SetContainer(IUnityContainer container)
        {
            _container = container;
            _globalVariables = _container.Resolve<IGlobalVariablesService>();
            _toast = _container.Resolve<IToastService>();
            _logger = _container.Resolve<ILogger>();
            _localization = _container.Resolve<ILocalizationManager>();
        }

        protected void ToastError(string message = "An error occurred.")
        {
            _toast.ShowToast(message, ToastType.Error);
        }

        protected void ToastSuccess(string message = "Operation completed successfully.")
        {
            _toast.ShowToast(message, ToastType.Success);
        }

        protected void ToastWarning(string message)
        {
            _toast.ShowToast(message, ToastType.Warning);
        }

        protected void ToastInformation(string message)
        {
            _toast.ShowToast(message, ToastType.Information);
        }

        /// <summary>
        /// Indicates that the code is running in VisualStudio Designer
        /// </summary>
        protected bool IsDesignMode()
        {
            return DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }

        protected object Resolve(Type type)
        {
            var result = _container.Resolve(type);
            if (result is IViewModel vm && vm.ParentViewModel == null)
                AddChildViewModel(vm);
            return result;
        }

        protected T Resolve<T>()
        {
            return (T) Resolve(typeof(T));
        }

        protected void LogError(string EventSource, Exception ex)
        {
            _logger.Log(EventSource, ex);
        }

        protected void LogError(Exception ex)
        {
            LogError(vmName, ex);
        }

        protected void RegisterInstance<T>(T instance)
        {
            _container.RegisterInstance<T>(instance);
        }

        protected object FindResource(string ResourceName)
        {
            return Application.Current.TryFindResource(ResourceName);
        }

        protected T FindResource<T>(string ResourceName)
        {
            return (T) Application.Current.TryFindResource(ResourceName);
        }

        public virtual void Dispose()
        {
            foreach (var child in ChildViewModels)
            {
                child.Dispose();
            }
        }

        protected virtual void SetRequiredFields(RequiredFieldCollection requiredFields)
        {
        }

        protected virtual void GetRequiredPropertiesInfo()
        {
            var type = GetType();
            foreach (var requiredField in RequiredFields)
            {
                if (requiredField.Item == null)
                    _requiredFieldsPropertyInfo[requiredField.FieldName] = type.GetProperty(requiredField.FieldName);
                else
                    _requiredFieldsPropertyInfo[requiredField.FieldName] = requiredField.Item.GetType()
                        .GetProperty(requiredField.FieldName);
            }
        }

        public string this[string columnName]
        {
            get { return CheckField(columnName); }
        }

        protected virtual string CheckField(string fieldName)
        {
            var field = RequiredFields?.FirstOrDefault(e => e.FieldName == fieldName);
            if (field != null && _requiredFieldsPropertyInfo.ContainsKey(field.FieldName))
            {
                var property = _requiredFieldsPropertyInfo[field.FieldName];
                if (property != null && field.ValidateField != null)
                {
                    if (field.Item == null)
                        return field.ValidateField(property.GetValue(this)) ? null : field.ErrorMessage;
                    return field.ValidateField(property.GetValue(field.Item)) ? null : field.ErrorMessage;
                }
            }
            return null;
        }

        public virtual IEnumerable<string> Validate()
        {
            var errors = new List<string>();
            if (RequiredFields != null)
            {
                foreach (var requiredField in RequiredFields)
                {
                    var error = CheckField(requiredField.FieldName);
                    if (!string.IsNullOrEmpty(error))
                        errors.Add(error);
                }
            }
            return errors;
        }
       
        public string Error => null;
    }

    /// <summary>
    /// The base class for ViewModels with a specific view
    /// Contains the property changed handlers and UI execute method
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    public class ViewModelBase<TView> : ViewModelBase, IPresentable<TView>, IPresentableViewModel
        where TView : UIElement, new()
    {
        private const string _eventsSource = "VMBase";

        public ViewModelBase()
        {
            ViewCreate += OnViewCreated;
        }

        protected virtual void OnViewCreated(object sender, TView e)
        {
            _logger?.Log(_eventsSource, $"View created {e.GetType().Name} for {vmName}", LogRecordTypes.Debug);
        }

        /// <summary>
        /// The view object
        /// </summary>
        protected TView _view;
        public TView View => (TView) GetView();

        public event EventHandler<TView> ViewCreate;

        /// <summary>
        /// Returns a the view object 
        /// Creates a new object on the first call
        /// </summary>
        /// <returns></returns>
        public virtual UIElement GetView()
        {
            if (_view == null)
                CreateView();
            return _view;
        }

        protected void CreateView()
        {
            ExecuteOnUIContext(() =>
            {
                _view = new TView();
                // Check if the view is a Framework element then
                // set the DataContext to this ViewModel
                var frameworkElement = _view as FrameworkElement;
                if (frameworkElement != null)
                    frameworkElement.DataContext = this;
                //frameworkElement.Loaded += delegate { };
                ViewCreate?.Invoke(this, _view);
            });
        }
    }

    public abstract class EditorViewModel<TEditor, TView, TModel> : ViewModelBase<TView>,
        IEditorViewModel<TView, TModel>
        where TView : UIElement, new()
        where TModel : new()
        where TEditor : EditorViewModel<TEditor, TView, TModel>

    {
        private const string _eventsSource = "EditorBase";

        protected EditorViewModel(IObjectMapper mapper)
        {
            _mapper = mapper;
            Saved += OnSave;
            Canceled += OnCanceled;
        }

        protected readonly IObjectMapper _mapper;
        public event EventHandler<EditorSaveArgs<TModel>> Saved;
        public event EventHandler<EditorSaveArgs<TModel>> Canceled;

        private OperationType _operationType;
        public OperationType OperationType
        {
            get { return _operationType; }
            set { SetProperty(ref this._operationType, value); }
        }

        private TModel _editedItem;
        public TModel EditedItem
        {
            get { return _editedItem; }
            set { SetProperty(ref this._editedItem, value); }
        }


        private bool _fillingValues;
        public bool FillingValues
        {
            get { return _fillingValues; }
            set { SetProperty(ref this._fillingValues, value); }
        }

        public virtual void Add()
        {
            OperationType = OperationType.Add;
            Clear();
            OnAdd();
        }

        public virtual void Clear()
        {
            EditedItem = new TModel();
        }

        public virtual void Edit(TModel item)
        {
            FillingValues = true;
            OperationType = OperationType.Edit;
            Clear();
            EditedItem = _mapper.Map<TModel>(item);
            OnEdit(item);
            FillingValues = false;
        }

        protected override async Task OnInitialization()
        {
            await base.OnInitialization();
            _mapper.RegisterMap<TModel, TModel>();
            _mapper.RegisterTwoWayMap<TEditor, TModel>();
        }

        protected virtual void OnAdd()
        {
        }

        protected virtual void OnEdit(TModel item)
        {
            MapEditedItemToEditor(item);
        }

        public virtual void MapEditedItemToEditor(TModel item)
        {
            _mapper.MapInstance(item, (TEditor)this);
        }

        public virtual void MapEditorToEditedItem(TModel item)
        {
            _mapper.MapInstance((TEditor)this, item);
        }

        public void Cancel()
        {
            Canceled?.Invoke(this, new EditorSaveArgs<TModel>(OperationType, EditedItem));
        }

        public virtual void PreSave()
        {
            MapEditorToEditedItem(EditedItem);
        }

        public virtual async Task<bool> Save()
        {
            try
            {
                var errors = Validate();
                if (errors != null && errors.Any())
                {
                    ShowErrors(errors);
                    return false;
                }

                PreSave();
                Saved?.Invoke(this, new EditorSaveArgs<TModel>(OperationType, await SaveItem()));
                return true;
            }
            catch (Exception ex)
            {
                _logger?.Log(_eventsSource, ex);
                return false;
            }
        }

        protected virtual Task<TModel> SaveItem()
        {
            return Task.FromResult(EditedItem);
        }

        protected virtual void ShowErrors(IEnumerable<string> errors)
        {
            var errorBuilder = new StringBuilder();
            foreach (var error in errors)
            {
                errorBuilder.Append(error);
                errorBuilder.Append(Environment.NewLine);
            }
            var errorMessage = errorBuilder.ToString(0, errorBuilder.Length - Environment.NewLine.Length);
            _toast.ShowToast(errorMessage, ToastType.Error);
        }

        public virtual void OnSave(object sender, EditorSaveArgs<TModel> e)
        {
        }

        public virtual void OnCanceled(object sender, EditorSaveArgs<TModel> e)
        {
        }
    }

    public abstract class EditorViewModel<TEditor, TView, TRepository, TModel,
        TKey> : EditorViewModel<TEditor, TView, TModel>
        where TView : UIElement, new()
        where TModel : class, IUniqueObject<TKey>, new()
        where TRepository : class, ICRUDRepository<TModel, TKey>
        where TEditor : EditorViewModel<TEditor, TView, TModel>
    {
        protected TRepository _repository;

        protected EditorViewModel(IObjectMapper mapper, TRepository repository) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<TModel> SaveItem()
        {
            return await _repository.Update(OperationType, EditedItem);
        }
    }
}