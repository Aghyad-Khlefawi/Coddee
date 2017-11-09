// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
    public abstract class ViewModelBase : BindableBase, IPresentableViewModel
    {
        private const string _eventsSource = "VMBase";


        protected static readonly Task completedTask = Task.FromResult(false);
        protected static WPFApplication _app;
        protected static IContainer _container;
        protected static IGlobalVariablesService _globalVariables;
        protected static IDialogService _dialogService;
        protected static IToastService _toast;
        protected static ILocalizationManager _localization;
        protected static ILogger _logger;
        protected static IRepositoryManager _repositoryManager;
        protected static IObjectMapper _mapper;
        protected static IViewModelsManager _vmManager;
        protected static IEventDispatcher _eventDispatcher;

        protected bool _validateOnPropertyChanged;
        public string __Name { get; protected set; }


        protected ViewModelBase()
        {
            RequiredFields = new RequiredFieldCollection();
            _requiredFieldsPropertyInfo = new Dictionary<string, PropertyInfo>();
            _views = new Dictionary<int, UIElement>();
            _viewsTypes = new Dictionary<int, Type>();

            __Name = GetType().Name;
            if (IsDesignMode())
                OnDesignMode();
            Errors = new List<string>();
        }

        protected UIElement _currentView;
        private readonly Dictionary<int, UIElement> _views;
        private readonly Dictionary<int, Type> _viewsTypes;
        public event EventHandler<UIElement> ViewCreated;

        private bool _viewsRegistered;

        public List<string> Errors { get; set; }
        public RequiredFieldCollection RequiredFields { get; }

        protected readonly Dictionary<string, PropertyInfo> _requiredFieldsPropertyInfo;

        public event ViewModelEventHandler Initialized;

        private bool _isValid;
        public bool IsValid
        {
            get { return _isValid; }
            protected set { SetProperty(ref this._isValid, value); }
        }

        private bool _isInitialized;
        public bool IsInitialized
        {
            get { return _isInitialized; }
            protected set { SetProperty(ref this._isInitialized, value); }
        }
        public string ViewModelGroup { get; private set; }

        private bool _isBusy = true;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref this._isBusy, value); }
        }
        private int _currentViewIndex;
        public int CurrentViewIndex
        {
            get { return _currentViewIndex; }
            set { SetProperty(ref _currentViewIndex, value); }
        }

        protected Task<IViewModel> InitializeViewModel(Type viewModelType)
        {
            return _vmManager.InitializeViewModel(viewModelType, this);
        }

        protected virtual void OnDesignMode()
        {
            IsBusy = false;
        }

        protected Task<TResult> InitializeViewModel<TResult>() where TResult : IViewModel
        {
            return _vmManager.InitializeViewModel<TResult>(this);
        }

        protected IEnumerable<IViewModel> GetChildViewModels()
        {
            return _vmManager.GetChildViewModels(this).Select(e => e.ViewModel);
        }

        protected IViewModel CreateViewModel(Type viewModelType)
        {
            return _vmManager.CreateViewModel(viewModelType, this);
        }

        protected TResult CreateViewModel<TResult>() where TResult : IViewModel
        {
            return _vmManager.CreateViewModel<TResult>(this);
        }

        protected Task InitializeChildViewModels(bool forceInitialize = false)
        {
            return GetChildViewModels().InitializeAll(forceInitialize);
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

        public void SetViewModelGroup(string group)
        {
            _vmManager.AddViewModelToGroup(group, this);
            ViewModelGroup = group;
        }

        /// <summary>
        /// Called when the ViewModel is ready to be presented
        /// </summary>
        /// <returns></returns>
        public Task Initialize(bool forceInitialize = false)
        {
            bool skip = false;
            lock (_initializationLock)
            {
                if (IsInitialized && !forceInitialize)
                {
                    skip = true;
                }
                IsInitialized = true;
            }

            if (skip)
            {
                _logger?.Log(_eventsSource, $"ViewModel {__Name} is initialized skipping initialization", LogRecordTypes.Debug);
                return completedTask;
            }

            _logger?.Log(_eventsSource, $"ViewModel {__Name} initializing", LogRecordTypes.Debug);
            return Task.Run(() =>
            {
                var task = OnInitialization().ContinueWith(t =>
                {
                    RefreshRequiredFields();
                    PropertyChanged += PropertyChangedHandler;
                    IsBusy = false;
                    OnInitialized(this);
                });
                Task.WaitAll(task);
            });
        }

        public virtual TView GetDefaultView<TView>(int index) where TView : UIElement
        {
            return (TView)GetView(index);
        }

        public virtual UIElement GetView()
        {
            return GetView(CurrentViewIndex);
        }

        public virtual UIElement GetView(int index)
        {
            if (!_viewsRegistered)
                RegisterViews();

            if (!_viewsTypes.ContainsKey(index))
                throw new ArgumentException($"There is no view with the index {index}");

            if (!_views.ContainsKey(index))
            {
                var type = _viewsTypes[index];
                CreateView(index, type);
            }
            return _currentView = _views[index];
        }

        protected virtual TView CreateView<TView>(int index) where TView : UIElement, new()
        {
            return (TView)CreateView(index, typeof(TView));
        }

        protected virtual UIElement CreateView(int index, Type viewType)
        {
            if (!_viewsRegistered)
                RegisterViews();
            UIElement view = null;
            ExecuteOnUIContext(() =>
            {
                view = (UIElement)Activator.CreateInstance(viewType);
                _views[index] = view;
                // Check if the view is a Framework element then
                // set the DataContext to this ViewModel
                var frameworkElement = view as FrameworkElement;
                if (frameworkElement != null)
                    frameworkElement.DataContext = this;
                //frameworkElement.Loaded += delegate { };
                OnViewCreated(view);
            });
            return view;
        }

        protected virtual void OnViewCreated(UIElement e)
        {
            _logger?.Log(_eventsSource, $"View created {e.GetType().Name} for {__Name}", LogRecordTypes.Debug);
            ViewCreated?.Invoke(this, e);
        }

        protected virtual void RegisterViews()
        {
            _viewsRegistered = true;
        }

        protected virtual void RegisterViewType<T>(int index) where T : UIElement
        {
            if (_views.ContainsKey(index))
                throw new ArgumentException($"There is already a view with the index {index}");
            _viewsTypes.Add(index, typeof(T));
        }

        protected virtual void PropertyChangedHandler(object sender, PropertyChangedEventArgs args)
        {
            if (_validateOnPropertyChanged && RequiredFields.Any(e => e.FieldName == args.PropertyName))
                Validate();
        }

        /// <summary>
        /// Gets a repository by its interface
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        protected TInterface GetRepository<TInterface>() where TInterface : IRepository
        {
            return _repositoryManager.GetRepository<TInterface>();
        }

        protected virtual void RefreshRequiredFields()
        {
            RequiredFields.Clear();
            SetRequiredFields(RequiredFields);
            GetRequiredPropertiesInfo();
        }

        protected virtual void OnInitialized(IViewModel sender)
        {
            _logger?.Log(_eventsSource, $"ViewModel {__Name} initialization completed ", LogRecordTypes.Debug);
            Initialized?.Invoke(this);
        }

        /// <summary>
        /// Set the container on the view model and resolve the basic dependencies
        /// </summary>
        /// <param name="container"></param>
        public static void SetContainer(IContainer container)
        {
            _container = container;

            if (_container.IsRegistered<IGlobalVariablesService>())
                _globalVariables = _container.Resolve<IGlobalVariablesService>();

            if (_container.IsRegistered<ILogger>())
                _logger = _container.Resolve<ILogger>();

            if (_container.IsRegistered<IViewModelsManager>())
                _vmManager = _container.Resolve<IViewModelsManager>();

            if (_container.IsRegistered<IToastService>())
                _toast = _container.Resolve<IToastService>();

            if (_container.IsRegistered<IDialogService>())
                _dialogService = _container.Resolve<IDialogService>();

            if (_container.IsRegistered<IRepositoryManager>())
                _repositoryManager = _container.Resolve<IRepositoryManager>();

            if (_container.IsRegistered<ILocalizationManager>())
                _localization = _container.Resolve<ILocalizationManager>();

            if (_container.IsRegistered<IObjectMapper>())
                _mapper = _container.Resolve<IObjectMapper>();

            if (_container.IsRegistered<IEventDispatcher>())
                _eventDispatcher = _container.Resolve<IEventDispatcher>();
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
            _logger.Log(__Name, ex);
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
                _logger.Log(__Name, $"Checking field {fieldName}", LogRecordTypes.Debug);
                var property = _requiredFieldsPropertyInfo[field.FieldName];
                if (property != null && field.ValidateField != null)
                {
                    try
                    {
                        if (field.Item == null)
                            return field.ValidateField(property.GetValue(this)) ? null : field.ErrorMessage;
                        return field.ValidateField(property.GetValue(field.Item)) ? null : field.ErrorMessage;
                    }
                    catch (Exception ex)
                    {
                        LogError($"{field.FieldName} Field validator", ex);
                        return "FieldValidationException";
                    }
                }
            }
            return null;
        }

        protected bool _validating;

        public IEnumerable<string> Validate(bool validateChildren = false)
        {
            Errors.Clear();

            if (_validating)
                return Errors;

            _validating = true;


            if (RequiredFields != null)
            {
                foreach (var requiredField in RequiredFields)
                {
                    var error = CheckField(requiredField.FieldName);
                    if (!string.IsNullOrEmpty(error))
                        Errors.Add(error);
                }
            }

            if (validateChildren)
            {
                foreach (var childViewModel in GetChildViewModels())
                {
                    Errors.AddRange(childViewModel.Validate(true));
                }
            }

            CustomValidation(Errors);
            IsValid = !Errors.Any();
            OnValidated(Errors);
            _validating = false;
            return Errors;
        }

        public virtual void CustomValidation(List<string> errors)
        {
        }

        protected void OnValidated(List<string> errors)
        {
            Validated?.Invoke(this, errors);
        }
        public event ViewModelEventHandler<IEnumerable<string>> Validated;

        public ReactiveCommand<ViewModelBase> CreateReactiveCommand(Action handler)
        {
            return ReactiveCommand<ViewModelBase>.Create(this, handler);
        }

        public ReactiveCommand<T> CreateReactiveCommand<T>(T obj, Action handler)
        {
            return ReactiveCommand<T>.Create(obj, handler);
        }
        public ReactiveCommand<T, TParam> CreateReactiveCommand<T, TParam>(T obj, Action<TParam> handler)
        {
            return ReactiveCommand<T, TParam>.Create(obj, handler);
        }

        protected virtual void RaiseEvent<TEvent, TArgs>(TArgs args)
            where TEvent : class, IViewModelEvent<TArgs>, new()
        {
            var targetEvent = _eventDispatcher.GetEvent<TEvent>();
            targetEvent.Raise(this, args);
        }
        protected virtual void RaiseGroupEvent<TEvent, TArgs>(string viewModelGroup, TArgs args)
            where TEvent : ViewModelsGroupEvent<TArgs>, new()
        {
            var targetEvent = _eventDispatcher.GetEvent<TEvent>();
            targetEvent.Raise(viewModelGroup, this, args);
        }
        protected virtual void RaiseGroupEvent<TEvent, TArgs>(TArgs args)
            where TEvent : ViewModelsGroupEvent<TArgs>, new()
        {
            if (string.IsNullOrEmpty(ViewModelGroup))
                throw new InvalidOperationException("ViewModelGroup is not set.");
            RaiseGroupEvent<TEvent, TArgs>(ViewModelGroup, args);
        }

        protected virtual void SubscribeToEvent<TEvent, TArgs>(
            ViewModelEventHandler<TArgs> handler)
            where TEvent : class, IViewModelEvent<TArgs>, new()
        {
            var targetEvent = _eventDispatcher.GetEvent<TEvent>();
            targetEvent.Subscribe(this, handler);
        }
        protected virtual void SubscribeToGroupEvent<TEvent, TArgs>(string viewModelGroup,
            ViewModelEventHandler<TArgs> handler)
            where TEvent : ViewModelsGroupEvent<TArgs>, new()
        {
            var targetEvent = _eventDispatcher.GetEvent<TEvent>();
            targetEvent.Subscribe(viewModelGroup, handler);
        }
        protected virtual void SubscribeToGroupEvent<TEvent, TArgs>(ViewModelEventHandler<TArgs> handler)
            where TEvent : ViewModelsGroupEvent<TArgs>, new()
        {
            if (string.IsNullOrEmpty(ViewModelGroup))
                throw new InvalidOperationException("ViewModelGroup is not set.");
            SubscribeToGroupEvent<TEvent, TArgs>(ViewModelGroup, handler);
        }
        protected virtual void ToggleBusy(Action action)
        {
            try
            {
                IsBusy = true;
                action?.Invoke();
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected virtual async Task ToggleBusyAsync(Task action)
        {
            try
            {
                IsBusy = true;
                await action;
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected virtual void ToViewModelEvent<TEvent, TParam>(ref ViewModelEventHandler<TParam> handler) where TEvent : ViewModelEvent<TParam>, new()
        {
            handler += (sender, args) =>
            {
                _eventDispatcher.GetEvent<TEvent>().Raise(this, args);
            };
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
        public TView View => (TView)GetView();


        protected override void RegisterViews()
        {
            base.RegisterViews();
            RegisterViewType<TView>(0);
        }


        protected override void OnViewCreated(UIElement e)
        {
            base.OnViewCreated(e);
            if (e is TView defaultView)
                OnDefaultViewCreated(defaultView);
        }

        protected TView CreateView()
        {
            return (TView)CreateView(0, typeof(TView));
        }

        protected virtual void OnDefaultViewCreated(TView view)
        {

        }
        public TView GetDefaultView()
        {
            return (TView)GetView(0);
        }
    }
}