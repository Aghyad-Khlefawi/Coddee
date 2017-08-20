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
using Coddee.Validation;
using Coddee.WPF.Commands;


namespace Coddee.WPF
{
    /// <summary>
    /// The base class for all ViewModels of the application
    /// Contains the property changed handlers and UI execute method
    /// </summary>
    public abstract class ViewModelBase : BindableBase, IViewModel
    {
        private const string _eventsSource = "VMBase";

        
        protected static readonly Task completedTask = Task.FromResult(false);
        protected static WPFApplication _app;
        protected static IContainer _container;
        protected static IGlobalVariablesService _globalVariables;
        protected static IToastService _toast;
        protected static ILocalizationManager _localization;
        protected static ILogger _logger;

        private static IViewModelsManager VmManager;

        public string __Name { get; protected set; }


        protected ViewModelBase()
        {
            Initialized += OnInitialized;
            RequiredFields = new RequiredFieldCollection();
            _requiredFieldsPropertyInfo = new Dictionary<string, PropertyInfo>();
            __Name = GetType().Name;
            if (IsDesignMode())
                OnDesignMode();
        }

        public RequiredFieldCollection RequiredFields { get; }

        protected readonly Dictionary<string, PropertyInfo> _requiredFieldsPropertyInfo;

        public event EventHandler Initialized;


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

        protected Task<IViewModel> InitializeViewModel(Type viewModelType)
        {
            return VmManager.InitializeViewModel(viewModelType, this);
        }

        protected virtual void OnDesignMode()
        {
            IsBusy = false;
        }

        protected Task<TResult> InitializeViewModel<TResult>() where TResult : IViewModel
        {
            return VmManager.InitializeViewModel<TResult>(this);
        }

        protected virtual Task IntitilzieChildViewModels()
        {
            return Task.WhenAll(VmManager.GetChildViewModels(this).Select(e => e.ViewModel.Initialize()));
        }

        protected IEnumerable<IViewModel> GetChildViewModels()
        {
            return VmManager.GetChildViewModels(this).Select(e => e.ViewModel);
        }
        protected IViewModel CreateViewModel(Type viewModelType)
        {
            return VmManager.CreateViewModel(viewModelType, this);
        }

        protected TResult CreateViewModel<TResult>() where TResult : IViewModel
        {
            return VmManager.CreateViewModel<TResult>(this);
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

        private readonly object _initializationLock = new object(); 

        /// <summary>
        /// Called when the ViewModel is ready to be presented
        /// </summary>
        /// <returns></returns>
        public Task Initialize()
        {
            lock (_initializationLock)
                IsInitialized = true;

            _logger?.Log(_eventsSource, $"ViewModel initializing {__Name}", LogRecordTypes.Debug);
            return Task.Run(() =>
            {
                OnInitialization().Wait();

                RefreshRequiredFields();

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
            _logger?.Log(_eventsSource, $"ViewModel initialized {__Name}", LogRecordTypes.Debug);
        }

        /// <summary>
        /// Set the container on the view model and resolve the basic dependencies
        /// </summary>
        /// <param name="container"></param>
        public static void SetContainer(IContainer container)
        {
            _container = container;
            _globalVariables = _container.Resolve<IGlobalVariablesService>();
            _logger = _container.Resolve<ILogger>();
            _localization = _container.Resolve<ILocalizationManager>();
            VmManager = _container.Resolve<IViewModelsManager>();

            if (_container.IsRegistered<IToastService>())
                _toast = _container.Resolve<IToastService>();

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
            return _container.Resolve(type);
        }

        protected T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        protected void LogError(string EventSource, Exception ex)
        {
            _logger.Log(EventSource, ex);
        }

        protected void LogError(Exception ex)
        {
            LogError(__Name, ex);
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
            return (T)Application.Current.TryFindResource(ResourceName);
        }

        public virtual void Dispose()
        {
            foreach (var child in GetChildViewModels())
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


        public ReactiveCommand<T> CreateReactiveCommand<T>(T obj, Action handler)
        {
            return ReactiveCommand<T>.Create(obj, handler);
        }
        public ReactiveCommand<T, TParam> CreateReactiveCommand<T, TParam>(T obj, Action<TParam> handler)
        {
            return ReactiveCommand<T, TParam>.Create(obj, handler);
        }
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
            _logger?.Log(_eventsSource, $"View created {e.GetType().Name} for {__Name}", LogRecordTypes.Debug);
        }

        /// <summary>
        /// The view object
        /// </summary>
        protected TView _view;
        public TView View => (TView)GetView();

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
                throw;
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