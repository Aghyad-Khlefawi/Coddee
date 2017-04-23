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
        }

        public event EventHandler Initialized;
        public event EventHandler<IViewModel> ChildCreated;

        public IViewModel ParentViewModel { get; set; }
        public IList<IViewModel> ChildViewModels { get; protected set; }
        public bool IsInitialized { get; protected set; }

        protected async Task<IViewModel> InitializeViewModel(Type viewModelType)
        {
            var vm = CreateViewModel(viewModelType);
            await vm.Initialize();
            return vm;
        }
        protected async Task<TResult> InitializeViewModel<TResult>() where TResult : IViewModel
        {
            var vm = CreateViewModel<TResult>();
            await vm.Initialize();
            return vm;
        }
        protected IViewModel CreateViewModel(Type viewModelType)
        {
            IViewModel vm = (IViewModel)Resolve(viewModelType);
            vm.ParentViewModel = this;
            (ChildViewModels ?? (ChildViewModels = new List<IViewModel>())).Add(vm);
            ChildCreated?.Invoke(this, vm);
            return vm;
        }
        protected TResult CreateViewModel<TResult>() where TResult : IViewModel
        {
            var vm = Resolve<TResult>();
            vm.ParentViewModel = this;
            (ChildViewModels ?? (ChildViewModels = new List<IViewModel>())).Add(vm);
            ChildCreated?.Invoke(this, vm);
            return vm;
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
        public Dispatcher GetDispatcher()
        {
            return IsDesignMode() ? Application.Current.Dispatcher : _app.Dispatcher;
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
        public async Task Initialize()
        {
            await OnInitialization();
            IsInitialized = true;
            Initialized?.Invoke(this, EventArgs.Empty);
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

        public object FindResource(string ResourceName)
        {
            return Application.Current.TryFindResource(ResourceName);
        }

        public T FindResource<T>(string ResourceName)
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
            return _view;
        }
    }

    public class EditorViewModel<TView, TModel> : ViewModelBase<TView>, IEditorViewModel<TView, TModel>
        where TView : UIElement, new() where TModel : new()

    {
        public EditorViewModel()
        {
            Saved += OnSave;
            Canceled += OnCanceled;
        }


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

        public void Add()
        {
            OperationType = OperationType.Add;
            EditedItem = new TModel();
        }

        public void Edit(TModel item)
        {
            OperationType = OperationType.Edit;
            EditedItem = item;
        }

        public void Cancel()
        {
        }

        public void Save()
        {
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
}