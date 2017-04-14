// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Coddee.Loggers;
using Coddee.Services;
using Coddee.WPF.Modules;
using Microsoft.Practices.Unity;

namespace Coddee.WPF
{
    /// <summary>
    /// The base class for all ViewModels of the application
    /// Contains the property changed handlers and UI execute method
    /// </summary>
    public class ViewModelBase : BindableBase
    {
        protected static readonly Task completedTask = Task.FromResult(false);
        protected static WPFApplication _app;
        protected static IUnityContainer _container;
        protected static IGlobalVariablesService _globalVariables;
        protected static IToastService _toast;
        protected static ILogger _logger;


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

        /// <summary>
        /// Called when the ViewModel is ready to be presented
        /// </summary>
        /// <returns></returns>
        public virtual Task Initialize()
        {
            return completedTask;
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
    }

    /// <summary>
    /// The base class for ViewModels with a specific view
    /// Contains the property changed handlers and UI execute method
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    public class ViewModelBase<TView> : ViewModelBase, IPresentable where TView : UIElement, new()
    {
        /// <summary>
        /// The view object
        /// </summary>
        protected TView _view;

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
                });
            return _view;
        }
    }
}