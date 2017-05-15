// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Coddee.Data;
using Coddee.Loggers;
using Coddee.Services;
using Coddee.WPF.Commands;
using Coddee.WPF.Modules;
using Microsoft.Practices.Unity;

namespace Coddee.WPF
{
    /// <summary>
    /// The base class for all ViewModels of the application
    /// Contains the property changed handlers and UI execute method
    /// </summary>
    public class ViewModelBase : BindableBase, IViewModel
    {
        protected static readonly Task completedTask = Task.FromResult(false);
        protected static WPFApplication _app;
        protected static IUnityContainer _container;
        protected static IGlobalVariablesService _globalVariables;
        protected static IToastService _toast;
        protected static ILogger _logger;

        public ViewModelBase()
        {
            Initialized += OnInitialized;
            ChildCreated += OnChildCreated;
            ChildViewModels = new List<IViewModel>();

            if (IsDesignMode())
                OnDesignMode();
        }

        
        public event EventHandler Initialized;
        public event EventHandler<IViewModel> ChildCreated;

        public IViewModel ParentViewModel { get; set; }

        public IList<IViewModel> ChildViewModels { get; protected set; }

        private bool _isInitialized ;
        public bool IsInitialized 
        {
            get { return _isInitialized ; }
            protected set { SetProperty(ref this._isInitialized , value); }
        }

        private bool _isBusy=true;
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
            vm.ParentViewModel = this;
            ChildViewModels.Add(vm);
            ChildCreated?.Invoke(this, vm);
            vm.ChildCreated += ChildCreated;
            return vm;
        }

        public TResult CreateViewModel<TResult>() where TResult : IViewModel
        {
            return (TResult) CreateViewModel(typeof(TResult));
        }

        protected virtual void OnChildCreated(object sender, IViewModel e)
        {
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
            return Task.Run(() =>
            {
                OnInitialization().Wait();
                IsInitialized = true;
                IsBusy = false;
                Initialized?.Invoke(this, EventArgs.Empty);
            });
        }

        protected virtual void OnInitialized(object sender, EventArgs e)
        {
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
            return _container.Resolve<T>();
        }

        protected void LogError(string EventSource, Exception ex)
        {
            _logger.Log(EventSource, ex);
        }

        protected void LogError(Exception ex)
        {
            LogError(GetType().Name, ex);
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
    }

    /// <summary>
    /// The base class for ViewModels with a specific view
    /// Contains the property changed handlers and UI execute method
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    public class ViewModelBase<TView> : ViewModelBase, IPresentable<TView> where TView : UIElement, new()
    {
        public ViewModelBase()
        {
            ViewCreate += OnViewCreated;
        }

        protected virtual void OnViewCreated(object sender, TView e)
        {
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
                ViewCreate?.Invoke(this, _view);
            });
        }
    }

    public abstract class EditorViewModel<TView, TModel> : ViewModelBase<TView>, IEditorViewModel<TView, TModel>
        where TView : UIElement, new() where TModel : new()

    {
        public EditorViewModel()
        {
            Saved += OnSave;
            Canceled += OnCanceled;
        }

        private IObjectMapper _mapper;
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

        public virtual void Add()
        {
            OperationType = OperationType.Add;
            EditedItem = new TModel();
            OnAdd();
        }

        public virtual void Edit(TModel item)
        {
            OperationType = OperationType.Edit;
            EditedItem = _mapper.Map<TModel>(item);
            OnEdit(item);
        }

        protected override Task OnInitialization()
        {
            _mapper = Resolve<IObjectMapper>();
            _mapper.RegisterMap<TModel, TModel>();
            return base.OnInitialization();
        }

        protected virtual void OnAdd()
        {
        }

        protected virtual void OnEdit(TModel item)
        {
        }

        public void Cancel()
        {
            Canceled?.Invoke(this, new EditorSaveArgs<TModel>(OperationType, EditedItem));
        }

        public virtual void PreSave()
        {
        }

        public virtual void Save()
        {
            PreSave();
            var errors = Validate();
            if (errors != null && errors.Any())
            {
                ShowErrors(errors);
            }
            else
            {
                Saved?.Invoke(this, new EditorSaveArgs<TModel>(OperationType, EditedItem));
            }
        }

        protected virtual void ShowErrors(IEnumerable<string> errors)
        {
        }

        public virtual void OnSave(object sender, EditorSaveArgs<TModel> e)
        {
        }

        public virtual void OnCanceled(object sender, EditorSaveArgs<TModel> e)
        {
        }

        public virtual IEnumerable<string> Validate()
        {
            return null;
        }
    }

    public class EditorViewModel<TView, TRepository, TModel, TKey> : EditorViewModel<TView, TModel>
        where TView : UIElement, new()
        where TModel : class, IUniqueObject<TKey>, new()
        where TRepository : class, ICRUDRepository<TModel, TKey>
    {
        protected TRepository _repository;
        public new event EventHandler<EditorSaveArgs<TModel>> Saved;

        protected override Task OnInitialization()
        {
            _repository = Resolve<TRepository>();
            return base.OnInitialization();
        }

        public override async void Save()
        {
            PreSave();
            var errors = Validate();
            if (errors != null && errors.Any())
            {
                ShowErrors(errors);
            }
            else
            {
                Saved?.Invoke(this,
                              new EditorSaveArgs<TModel>(OperationType,
                                                         await _repository.Update(OperationType, EditedItem)));
            }
        }
    }
}