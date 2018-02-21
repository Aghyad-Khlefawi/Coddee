// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Coddee.Data;
using Coddee.Exceptions;
using Coddee.Loggers;
using Coddee.Services;
using Coddee.Validation;
using Coddee.WPF.Commands;
using Coddee.Mvvm;

namespace Coddee.WPF
{
    /// <summary>
    /// The base class for all ViewModels of the application
    /// Contains the property changed handlers and UI execute method
    /// </summary>
    public abstract class ViewModelBase : UniversalViewModelBase
    {
        private const string _eventsSource = "VMBase";

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        protected static IRepositoryManager _repositoryManager;
        protected static IDialogService _dialogService;
        protected static IToastService _toast;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <inheritdoc />
        protected ViewModelBase()
        {
            if (IsDesignMode())
                OnDesignMode();
        }

        /// <inheritdoc />
        protected override void OnViewCreated(object view)
        {
            if (view is FrameworkElement frameworkElement)
                frameworkElement.DataContext = this;

            base.OnViewCreated(view);

        }

        /// <summary>
        /// Returns the application Dispatcher
        /// </summary>
        protected Dispatcher GetDispatcher()
        {
            return Application.Current.Dispatcher;
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
        /// Set the container on the view model and resolve the basic dependencies
        /// </summary>
        public new static void SetContainer(IContainer container)
        {

            if (_container.IsRegistered<IToastService>())
                _toast = _container.Resolve<IToastService>();

            if (_container.IsRegistered<IDialogService>())
                _dialogService = _container.Resolve<IDialogService>();

            if (_container.IsRegistered<IRepositoryManager>())
                _repositoryManager = _container.Resolve<IRepositoryManager>();
        }

        /// <summary>
        /// Shows a test message
        /// </summary>
        protected void ToastError(string message = "An error occurred.")
        {
            _toast?.ShowToast(message, ToastType.Error);
        }

        /// <summary>
        /// Shows a test message
        /// </summary>
        protected void ToastSuccess(string message = "Operation completed successfully.")
        {
            _toast?.ShowToast(message, ToastType.Success);
        }

        /// <summary>
        /// Shows a test message
        /// </summary>
        protected void ToastWarning(string message)
        {
            _toast?.ShowToast(message, ToastType.Warning);
        }

        /// <summary>
        /// Shows a test message
        /// </summary>
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


        /// <summary>
        /// Finds an XAML resource by name
        /// </summary>
        protected object FindResource(string ResourceName)
        {
            return Application.Current.TryFindResource(ResourceName);
        }

        /// <summary>
        /// Finds an XAML resource by name
        /// </summary>
        protected T FindResource<T>(string ResourceName)
        {
            return (T)Application.Current.TryFindResource(ResourceName);
        }


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


    }

    /// <summary>
    /// The base class for ViewModels with a specific view
    /// Contains the property changed handlers and UI execute method
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    public class ViewModelBase<TView> : ViewModelBase, IPresentable<TView>
        where TView : UIElement, new()
    {

        /// <summary>
        /// <inheritdoc cref="View"/>
        /// </summary>
        protected TView _view;

        /// <summary>
        /// The default view
        /// </summary>
        public TView View
        {
            get
            {
                if (_view == null)
                    ExecuteOnUIContext(() => { _view = GetDefaultView(); });
                return _view;
            }
            set { SetProperty(ref _view, value); }
        }

        /// <inheritdoc />
        protected override void RegisterViews()
        {
            base.RegisterViews();
            RegisterViewType<TView>(0);
        }

        /// <inheritdoc />
        protected override void OnViewCreated(object view)
        {
            base.OnViewCreated(view);
            if (view is TView defaultView)
                OnDefaultViewCreated(defaultView);
        }

        /// <summary>
        /// Create the default view
        /// </summary>
        protected TView CreateView()
        {
            return (TView)CreateView(0, typeof(TView));
        }

        /// <summary>
        /// Called when the default View is called
        /// </summary>
        /// <param name="view"></param>
        protected virtual void OnDefaultViewCreated(TView view)
        {
        }

        /// <summary>
        /// Returns the default View of the ViewModel
        /// </summary>
        /// <returns></returns>
        public TView GetDefaultView()
        {
            return (TView)GetView(0);
        }
    }
}