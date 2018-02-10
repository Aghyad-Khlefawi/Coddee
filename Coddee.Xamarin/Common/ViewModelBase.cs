using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Coddee.Data;
using Coddee.Exceptions;
using Coddee.Loggers;
using Coddee.Services;
using Coddee.Validation;
using Coddee.Xamarin.AppBuilder;
using Coddee.Xamarin.Commands;
using Coddee.Xamarin.Services.ViewModelManager;
using Xamarin.Forms;

namespace Coddee.Xamarin.Common
{
    /// <summary>
    /// The base class for all ViewModels of the application
    /// Contains the property changed handlers and UI execute method
    /// </summary>
    public abstract class ViewModelBase : BindableBase, IViewModel
    {
        private const string _eventsSource = "VMBase";
        protected static readonly Task completedTask = Task.FromResult(false);
        protected static XamarinApplication _app;
        protected static IContainer _container;
        protected static IGlobalVariablesService _globalVariables;
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
            ValidationRules = new List<IValidationRule>();
            _pages = new Dictionary<int, Page>();
            _pagesTypes = new Dictionary<int, Type>();

            __Name = GetType().Name;
        }

        protected Page _currentPage;
        private readonly Dictionary<int, Page> _pages;
        private readonly Dictionary<int, Type> _pagesTypes;
        public event EventHandler<Page> PageCreated;

        private bool _pagesRegistered;
        /// <summary>
        /// The validation rules set on this view model
        /// The rules should be added to the list by overriding the <see cref="SetValidationRules"/> method.
        /// </summary>
        public List<IValidationRule> ValidationRules { get; }

        /// <summary>
        /// The result of the last validation operation
        /// </summary>
        public ValidationResult ValidationResult { get; private set; }
        /// <summary>
        /// Triggered when the initialization function is completed
        /// </summary>
        public event ViewModelEventHandler Initialized;

        /// <summary>
        /// Triggered after the <see cref="SetValidationRules"/> method is called.
        /// </summary>
        public event ViewModelEventHandler<IEnumerable<IValidationRule>> ValidationRulesSet;

        /// <summary>
        /// A ViewModelOptions object set when calling <see cref="IViewModelsManager.CreateViewModel(System.Type,IViewModel)"/> to change the ViewModel behavior
        /// </summary>
        public virtual ViewModelOptions ViewModelOptions { get; private set; }

        /// <summary>
        /// The IsValid value of last validation operation
        /// </summary>
        private bool _isValid;
        public bool IsValid
        {
            get { return _isValid; }
            protected set { SetProperty(ref this._isValid, value); }
        }
        private bool _isInitialized;

        /// <summary>
        /// Indicates that the ViewModel Initialize method has completed
        /// </summary>
        public bool IsInitialized
        {
            get { return _isInitialized; }
            protected set { SetProperty(ref this._isInitialized, value); }
        }
        public virtual ViewModelOptions DefaultViewModelOptions => ViewModelOptions.Default;

        /// <summary>
        /// The ViewModel group
        /// Used for <see cref="ViewModelsGroupEvent{TPayload}"/> events
        /// </summary>
        public string ViewModelGroup { get; private set; }

        private bool _isBusy = true;
        /// <summary>
        /// Indicates that the ViewModel is doing some work and should not be interacted with
        /// <remarks>Should show a busy indicator</remarks>
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref this._isBusy, value); }
        }

        private int _currentPageIndex;
        /// <summary>
        /// The index of the currently displayed page
        /// </summary>
        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set
            {
                SetProperty(ref _currentPageIndex, value);
                PageIndexChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Triggered when the <see cref="CurrentPageIndex"/> property is changed
        /// </summary>
        public event ViewModelEventHandler<int> PageIndexChanged;


        /// <summary>
        /// Set the <see cref="ViewModelOptions"/> property
        /// </summary>
        /// <param name="options"></param>
        public virtual void SetViewModelOptions(ViewModelOptions options)
        {
            ViewModelOptions = options;
        }

        /// <summary>
        /// Called by the visual studio designer on design-time only
        /// </summary>
        protected virtual void OnDesignMode()
        {
            IsBusy = false;
        }


        /// <summary>
        /// Creates an initializes a ViewModel
        /// </summary>
        protected Task<IViewModel> InitializeViewModel(Type viewModelType)
        {
            return _vmManager.InitializeViewModel(viewModelType, this);
        }

        /// <summary>
        /// Creates an initializes a ViewModel
        /// </summary>
        protected Task<TResult> InitializeViewModel<TResult>() where TResult : IViewModel
        {
            return _vmManager.InitializeViewModel<TResult>(this);
        }

        /// <summary>
        /// Gets the child ViewModels that are created by this ViewModel
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<IViewModel> GetChildViewModels()
        {
            return _vmManager.GetChildViewModels(this).Select(e => e.ViewModel);
        }

        /// <summary>
        /// Creates a child ViewModel
        /// </summary>
        protected IViewModel CreateViewModel(Type viewModelType)
        {
            return _vmManager.CreateViewModel(viewModelType, this);
        }

        /// <summary>
        /// Creates a child ViewModel
        /// </summary>
        protected IViewModel CreateViewModel(Type viewModelType, ViewModelOptions viewModelOptions)
        {
            return _vmManager.CreateViewModel(viewModelType, this, viewModelOptions);
        }

        /// <summary>
        /// Creates a child ViewModel
        /// </summary>
        protected TResult CreateViewModel<TResult>() where TResult : IViewModel
        {
            return _vmManager.CreateViewModel<TResult>(this);
        }

        /// <summary>
        /// Creates a child ViewModel
        /// </summary>
        protected TResult CreateViewModel<TResult>(ViewModelOptions viewModelOptions) where TResult : IViewModel
        {
            return _vmManager.CreateViewModel<TResult>(this, viewModelOptions);
        }

        /// <summary>
        /// Calls the initialize method on all child ViewModels
        /// </summary>
        /// <param name="forceInitialize"></param>
        /// <returns></returns>
        protected Task InitializeChildViewModels(bool forceInitialize = false)
        {
            return GetChildViewModels().InitializeAll(forceInitialize);
        }


        /// <summary>
        /// Return the running application instance
        /// </summary>
        /// <param name="app"></param>
        public static void SetApplication(XamarinApplication app)
        {
            _app = app;
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
        /// Set the <see cref="ViewModelGroup"/> property
        /// </summary>
        /// <param name="group"></param>
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
            lock (_initializationLock)
            {
                _logger?.Log(_eventsSource, $"ViewModel {__Name} initializing", LogRecordTypes.Debug);
                bool skip = IsInitialized && !forceInitialize;
                IsInitialized = true;
                if (skip)
                {
                    _logger?.Log(_eventsSource, $"ViewModel {__Name} is initialized skipping initialization",
                        LogRecordTypes.Debug);
                    return completedTask;
                }
            }

            return Task.Run(OnInitialization).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    var initializationException = new InitializationException(_eventsSource, t.Exception);
                    LogError(_eventsSource, initializationException);
                    _logger.Log(_eventsSource, $"Failed to initialize ViewModel {__Name} .", LogRecordTypes.Error);
                }

                RefreshValidationRules();
                PropertyChanged += PropertyChangedHandler;
                IsBusy = false;
                OnInitialized(this);
            });
        }


        /// <summary>
        /// Get the default Page
        /// </summary>
        public virtual TPage GetDefaultPage<TPage>(int index) where TPage : Page
        {
            return (TPage) GetPage(index);
        }

        /// <summary>
        /// Get the page specified by <see cref="CurrentPageIndex"/>
        /// </summary>
        public virtual Page GetPage()
        {
            return GetPage(CurrentPageIndex);
        }


        /// <summary>
        /// Get page by index
        /// </summary>
        public virtual Page GetPage(int index)
        {
            if (!_pagesRegistered)
                RegisterPages();

            if (!_pagesTypes.ContainsKey(index))
                throw new ArgumentException($"There is no page with the index {index}");

            if (!_pages.ContainsKey(index))
            {
                var type = _pagesTypes[index];
                CreatePage(index, type);
            }

            return _currentPage = _pages[index];
        }

        /// <summary>
        /// Create page by index
        /// </summary>
        protected virtual TPage CreatePage<TPage>(int index) where TPage : Page, new()
        {
            return (TPage) CreatePage(index, typeof(TPage));
        }

        /// <summary>
        /// Create the view object
        /// </summary>
        protected virtual Page CreatePage(int index, Type pageType)
        {
            if (!_pagesRegistered)
                RegisterPages();
            Page page = null;

            ExecuteOnUIContext(() =>
            {
                page = (Page) Activator.CreateInstance(pageType);
                _pages[index] = page;
                // set the BindingContext to this ViewModel
                page.BindingContext = this;
                OnPageCreated(page);
            });
            return page;
        }

        /// <summary>
        /// Called when a page object is created
        /// </summary>
        protected virtual void OnPageCreated(Page page)
        {
            _logger?.Log(_eventsSource, $"Page created {page.GetType().Name} for {__Name}", LogRecordTypes.Debug);
            PageCreated?.Invoke(this, page);
        }

        /// <summary>
        /// Called to register the Pages this ViewModel can use
        /// </summary>
        protected virtual void RegisterPages()
        {
            _pagesRegistered = true;
        }

        /// <summary>
        /// Register a Page type this ViewModel can use
        /// </summary>
        protected virtual void RegisterPageType<T>(int index) where T : Page
        {
            if (_pages.ContainsKey(index))
                throw new ArgumentException($"There is already a page with the index {index}");
            _pagesTypes.Add(index, typeof(T));
        }

        /// <summary>
        /// Default IPropertyChanged handler
        /// </summary>
        protected virtual void PropertyChangedHandler(object sender, PropertyChangedEventArgs args)
        {
            if (_validateOnPropertyChanged && ValidationRules.Any(e => e.FieldName == args.PropertyName))
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

        /// <summary>
        /// Clears the current <see cref="ValidationRules"/> and calls <see cref="SetValidationRules"/>
        /// </summary>
        protected virtual void RefreshValidationRules()
        {
            ValidationRules.Clear();
            SetValidationRules(ValidationRules);
            ValidationRulesSet?.Invoke(this, ValidationRules);
        }

        /// <summary>
        /// Called when <see cref="OnInitialization"/> method is completed
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void OnInitialized(IViewModel sender)
        {
            _logger?.Log(_eventsSource, $"ViewModel {__Name} initialization completed ", LogRecordTypes.Debug);
            Initialized?.Invoke(this);
        }

        /// <summary>
        /// Set the container on the view model and resolve the basic dependencies
        /// </summary>
        public static void SetContainer(IContainer container)
        {
            _container = container;

            if (_container.IsRegistered<IGlobalVariablesService>())
                _globalVariables = _container.Resolve<IGlobalVariablesService>();

            if (_container.IsRegistered<ILogger>())
                _logger = _container.Resolve<ILogger>();

            if (_container.IsRegistered<IViewModelsManager>())
                _vmManager = _container.Resolve<IViewModelsManager>();

            if (_container.IsRegistered<IRepositoryManager>())
                _repositoryManager = _container.Resolve<IRepositoryManager>();

            if (_container.IsRegistered<ILocalizationManager>())
                _localization = _container.Resolve<ILocalizationManager>();

            if (_container.IsRegistered<IObjectMapper>())
                _mapper = _container.Resolve<IObjectMapper>();

            if (_container.IsRegistered<IEventDispatcher>())
                _eventDispatcher = _container.Resolve<IEventDispatcher>();
        }

        /// <summary>
        /// Resolve a dependency
        /// </summary>
        protected object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        /// <summary>
        /// Resolve a dependency
        /// </summary>
        protected T Resolve<T>()
        {
            return (T) Resolve(typeof(T));
        }

        /// <summary>
        /// Log an exception using the registered<see cref="ILogger"/>
        /// </summary>
        protected void LogError(string EventSource, Exception ex)
        {
            _logger.Log(__Name, ex);
        }

        /// <summary>
        /// Log an exception using the registered<see cref="ILogger"/>
        /// </summary>
        protected void LogError(Exception ex)
        {
            LogError(__Name, ex);
        }

        /// <summary>
        /// Register a dependency instance
        /// </summary>
        protected void RegisterInstance<T>(T instance)
        {
            _container.RegisterInstance<T>(instance);
        }

        /// <summary>
        /// Finds an XAML resource by name
        /// </summary>
        protected object FindResource(string ResourceName)
        {
            Application.Current.Resources.TryGetValue(ResourceName, out var resource);
            return resource;
        }

        /// <summary>
        /// Finds an XAML resource by name
        /// </summary>
        protected T FindResource<T>(string ResourceName)
        {
            Application.Current.Resources.TryGetValue(ResourceName, out var resource);
            return (T) resource;
        }


        /// <summary>
        /// Dispose from the ViewModel and remove it from ViewModel hierarchy
        /// </summary>
        public virtual void Dispose()
        {
            DisposeChildren();
            _vmManager.RemoveViewModel(this);
        }

        /// <summary>
        /// Calls the <see cref="Dispose"/> method on the child ViewModels
        /// </summary>
        protected virtual void DisposeChildren()
        {
            foreach (var child in GetChildViewModels().ToList())
            {
                child.Dispose();
            }
        }

        /// <summary>
        /// Set the ViewModel validation rules
        /// </summary>
        protected virtual void SetValidationRules(List<IValidationRule> validationRules)
        {
        }

        protected bool _validating;

        /// <summary>
        /// Validate the ViewModel according to the <see cref="ValidationRules"/>
        /// </summary>
        public ValidationResult Validate(bool validateChildren = false)
        {
            ValidationResult = new ValidationResult();

            if (_validating)
                return ValidationResult;

            _validating = true;

            if (validateChildren)
            {
                foreach (var childViewModel in GetChildViewModels())
                {
                    if (!childViewModel.ViewModelOptions.IncludeInHierarchicalValidation)
                        continue;

                    var result = childViewModel.Validate(true);
                    if (result != null)
                        ValidationResult.Append(result);
                }
            }

            if (ValidationRules != null)
            {
                foreach (var validationRule in ValidationRules)
                {
                    CheckRule(validationRule);
                }
            }

            IsValid = ValidationResult.IsValid;
            OnValidated(ValidationResult);
            _validating = false;
            return ValidationResult;
        }

        /// <summary>
        /// Check if validation rule is valid
        /// </summary>
        private void CheckRule(IValidationRule validationRule)
        {
            if (!validationRule.Validate())
            {
                if (validationRule.ValidationType == ValidationType.Error)
                    ValidationResult.Errors.Add(validationRule.GetMessage());
                else
                    ValidationResult.Warnings.Add(validationRule.GetMessage());
            }
        }

        /// <summary>
        /// Called after the validation operation is completed
        /// </summary>
        protected void OnValidated(ValidationResult res)
        {
            Validated?.Invoke(this, res);
        }

        /// <summary>
        /// Triggered after the validation operation is completed
        /// </summary>
        public event ViewModelEventHandler<ValidationResult> Validated;

        /// <summary>
        /// Create a <see cref="ReactiveCommand{TObserved}"/> observing this ViewModel
        /// </summary>
        public ReactiveCommand<ViewModelBase> CreateReactiveCommand(Action handler)
        {
            return ReactiveCommand<ViewModelBase>.Create(this, handler);
        }

        /// <summary>
        /// Create a <see cref="ReactiveCommand{TObserved}"/> observing this ViewModel
        /// </summary>
        public ReactiveCommand<T> CreateReactiveCommand<T>(T obj, Action handler)
        {
            return ReactiveCommand<T>.Create(obj, handler);
        }

        /// <summary>
        /// Create a <see cref="ReactiveCommand{TObserved}"/> observing this ViewModel
        /// </summary>
        public ReactiveCommand<T, TParam> CreateReactiveCommand<T, TParam>(T obj, Action<TParam> handler)
        {
            return ReactiveCommand<T, TParam>.Create(obj, handler);
        }

        /// <summary>
        /// Calls the <see cref="IViewModelEvent{TPayload}.Raise"/> for an event
        /// </summary>
        protected virtual void RaiseEvent<TEvent, TArgs>(TArgs args)
            where TEvent : class, IViewModelEvent<TArgs>, new()
        {
            var targetEvent = _eventDispatcher.GetEvent<TEvent>();
            targetEvent.Raise(this, args);
        }


        /// <summary>
        /// Calls the <see cref="ViewModelsGroupEvent{TPayload}.Raise"/> for an ViewModelGroup event
        /// </summary>
        protected virtual void RaiseGroupEvent<TEvent, TArgs>(string viewModelGroup, TArgs args)
            where TEvent : ViewModelsGroupEvent<TArgs>, new()
        {
            var targetEvent = _eventDispatcher.GetEvent<TEvent>();
            targetEvent.Raise(viewModelGroup, this, args);
        }

        /// <summary>
        /// Calls the <see cref="ViewModelsGroupEvent{TPayload}.Raise"/> for an ViewModelGroup event
        /// </summary>
        protected virtual Task RaiseGroupEventAsync<TEvent, TArgs>(string viewModelGroup, TArgs args)
            where TEvent : ViewModelsGroupEvent<TArgs>, new()
        {
            var targetEvent = _eventDispatcher.GetEvent<TEvent>();
            return targetEvent.RaiseAsync(viewModelGroup, this, args);
        }

        /// <summary>
        /// Calls the <see cref="ViewModelsGroupEvent{TPayload}.Raise"/> for an ViewModelGroup event
        /// </summary>
        protected virtual void RaiseGroupEvent<TEvent, TArgs>(TArgs args)
            where TEvent : ViewModelsGroupEvent<TArgs>, new()
        {
            if (string.IsNullOrEmpty(ViewModelGroup))
                throw new InvalidOperationException("ViewModelGroup is not set.");
            RaiseGroupEvent<TEvent, TArgs>(ViewModelGroup, args);
        }

        /// <summary>
        /// Calls the <see cref="ViewModelsGroupEvent{TPayload}.Raise"/> for an ViewModelGroup event
        /// </summary>
        protected virtual Task RaiseGroupEventAsync<TEvent, TArgs>(TArgs args)
            where TEvent : ViewModelsGroupEvent<TArgs>, new()
        {
            if (string.IsNullOrEmpty(ViewModelGroup))
                throw new InvalidOperationException("ViewModelGroup is not set.");
            return RaiseGroupEventAsync<TEvent, TArgs>(ViewModelGroup, args);
        }

        /// <summary>
        /// Subscribes to an <see cref="IViewModelEvent{TPayload}"/> event
        /// </summary>
        protected virtual void SubscribeToEvent<TEvent, TArgs>(ViewModelEventHandler<TArgs> handler)
            where TEvent : class, IViewModelEvent<TArgs>, new()
        {
            var targetEvent = _eventDispatcher.GetEvent<TEvent>();
            targetEvent.Subscribe(this, handler);
        }

        /// <summary>
        /// Subscribes to an <see cref="ViewModelsGroupEvent{TPayload}"/> event
        /// </summary>
        protected virtual void SubscribeToGroupEvent<TEvent, TArgs>(string viewModelGroup,
            ViewModelEventHandler<TArgs> handler)
            where TEvent : ViewModelsGroupEvent<TArgs>, new()
        {
            var targetEvent = _eventDispatcher.GetEvent<TEvent>();
            targetEvent.Subscribe(viewModelGroup, handler);
        }

        /// <summary>
        /// Subscribes to an <see cref="ViewModelsGroupEvent{TPayload}"/> event
        /// </summary>
        protected virtual void SubscribeToGroupEvent<TEvent, TArgs>(ViewModelEventHandler<TArgs> handler)
            where TEvent : ViewModelsGroupEvent<TArgs>, new()
        {
            if (string.IsNullOrEmpty(ViewModelGroup))
                throw new InvalidOperationException("ViewModelGroup is not set.");
            SubscribeToGroupEvent<TEvent, TArgs>(ViewModelGroup, handler);
        }

        /// <summary>
        /// Subscribes to an <see cref="ViewModelsGroupEvent{TPayload}"/> event
        /// </summary>
        protected virtual void SubscribeToGroupEventAsync<TEvent, TArgs>(string viewModelGroup,
            AsyncViewModelEventHandler<TArgs> handler)
            where TEvent : ViewModelsGroupEvent<TArgs>, new()
        {
            var targetEvent = _eventDispatcher.GetEvent<TEvent>();
            targetEvent.SubscribeAsync(viewModelGroup, handler);
        }

        /// <summary>
        /// Subscribes to an <see cref="ViewModelsGroupEvent{TPayload}"/> event
        /// </summary>
        protected virtual void SubscribeToGroupEventAsync<TEvent, TArgs>(AsyncViewModelEventHandler<TArgs> handler)
            where TEvent : ViewModelsGroupEvent<TArgs>, new()
        {
            if (string.IsNullOrEmpty(ViewModelGroup))
                throw new InvalidOperationException("ViewModelGroup is not set.");
            SubscribeToGroupEventAsync<TEvent, TArgs>(ViewModelGroup, handler);
        }

        /// <summary>
        /// Setts the <see cref="IsBusy"/> property to true duration an action
        /// </summary>
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

        /// <summary>
        /// Setts the <see cref="IsBusy"/> property to true duration a task
        /// </summary>
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
    }


    /// <summary>
    /// The base class for ViewModels with a specific page
    /// Contains the property changed handlers and UI execute method
    /// </summary>
    /// <typeparam name="TPage"></typeparam>
    public class ViewModelBase<TPage> : ViewModelBase, IPresentable<TPage>
        where TPage : Page, new()
    {
        protected TPage _page;

        /// <summary>
        /// The default page
        /// </summary>
        public TPage Page
        {
            get
            {
                if (_page == null)
                    ExecuteOnUIContext(() => { _page = GetDefaultPage(); });
                return _page;
            }
            set { SetProperty(ref _page, value); }
        }

        protected override void RegisterPages()
        {
            base.RegisterPages();
            RegisterPageType<TPage>(0);
        }

        protected override void OnPageCreated(Page page)
        {
            base.OnPageCreated(page);
            if (page is TPage defaultPage)
                OnDefaultPageCreated(defaultPage);
        }

        /// <summary>
        /// Create the default page
        /// </summary>
        protected TPage CreatePage()
        {
            return (TPage) CreatePage(0, typeof(TPage));
        }

        /// <summary>
        /// Called when the default Page is called
        /// </summary>
        /// <param name="page"></param>
        protected virtual void OnDefaultPageCreated(TPage page)
        {
        }

        public TPage GetDefaultPage()
        {
            return (TPage) GetPage(0);
        }
    }
}