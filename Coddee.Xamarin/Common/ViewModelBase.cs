using System;
using Coddee.Data;
using Coddee.Mvvm;
using Coddee.Xamarin.Commands;
using Xamarin.Forms;

namespace Coddee.Xamarin.Common
{
    /// <summary>
    /// The base class for all ViewModels of the application
    /// Contains the property changed handlers and UI execute method
    /// </summary>
    public abstract class ViewModelBase : UniversalViewModelBase
    {
        private const string _eventsSource = "VMBase";
        protected static IRepositoryManager _repositoryManager;


        /// <inheritdoc />
        protected override void OnViewCreated(object view)
        {
            if (view is VisualElement visualElement)
                visualElement.BindingContext = this;
            base.OnViewCreated(view);
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
            if (_container.IsRegistered<IRepositoryManager>())
                _repositoryManager = _container.Resolve<IRepositoryManager>();
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
        where TView : Page, new()
    {
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

        protected override void RegisterViews()
        {
            base.RegisterViews();
            RegisterViewType<TView>(0);
        }

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
            return (TView) CreateView(0, typeof(TView));
        }

        /// <summary>
        /// Called when the default View is called
        /// </summary>
        /// <param name="view"></param>
        protected virtual void OnDefaultViewCreated(TView view)
        {
        }

        public TView GetDefaultView()
        {
            return (TView) GetView(0);
        }
    }
}