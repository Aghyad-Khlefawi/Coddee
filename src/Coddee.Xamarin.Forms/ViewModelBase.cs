// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using Coddee.Commands;
using Coddee.Mvvm;
using Xamarin.Forms;

namespace Coddee.Xamarin.Forms
{
    /// <summary>
    /// The base class for all ViewModels of the application
    /// Contains the property changed handlers and UI execute method
    /// </summary>
    public abstract class ViewModelBase : UniversalViewModelBase
    {
        private const string _eventsSource = "VMBase";

        /// <inheritdoc />
        protected override void OnViewCreated(object view)
        {
            if (view is VisualElement frameworkElement)
                frameworkElement.BindingContext = this;

            base.OnViewCreated(view);
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
        where TView : VisualElement, new()
    {

        /// <summary>
        /// The view navigation object.
        /// </summary>
        protected INavigation Navigation
        {
            get
            {
                return View.Navigation;
            }
        }

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

        protected virtual void NavigationPush(Page page)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PushAsync(page);
            });
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