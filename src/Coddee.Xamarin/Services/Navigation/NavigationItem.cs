using System;
using System.Threading.Tasks;
using Coddee.Mvvm;
using Coddee.Xamarin.Common;

namespace Coddee.Xamarin.Services.Navigation
{
    /// <summary>
    /// A navigation item for navigating application sections.
    /// </summary>
    public class NavigationItem : ViewModelBase, INavigationItem
    {
        /// <inheritdoc />
        public NavigationItem(
            IPresentable destination,
            string title,
            string icon,
            NavItemPosition position = NavItemPosition.Top,
            bool localizedTitle = false)
        {
            _destination = destination;
            Title = title;
            Icon = icon;
            Position = position;
            DestinationType = _destination?.GetType();
            DestinationResolved = true;
            _localizedTitle = localizedTitle;
        }

        /// <summary>
        /// The title of the item after localization.
        /// </summary>
        protected bool _localizedTitle;

        private bool _isVisible = true;
        /// <inheritdoc />
        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref this._isVisible, value); }
        }
        /// <inheritdoc />
        public event EventHandler ContentInitialized;
        /// <inheritdoc />
        public Type DestinationType { get; protected set; }
        /// <inheritdoc />
        public bool DestinationResolved { get; protected set; }

        /// <inheritdoc />
        public event EventHandler<IPresentable> NavigationRequested;

        /// <summary>
        /// the instance of the destination.
        /// </summary>
        protected IPresentable _destination;

        private NavItemPosition _position;

        /// <summary>
        /// The position of the item in the navigation bar.
        /// </summary>
        public NavItemPosition Position
        {
            get { return _position; }
            set { SetProperty(ref this._position, value); }
        }


        private bool _isSelected;
        /// <inheritdoc />
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref this._isSelected, value); }
        }
        private string _title;
        /// <summary>
        /// The title of the navigation item.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref this._title, value); }
        }

        private bool _showTitle;
        /// <inheritdoc />
        public bool ShowTitle
        {
            get { return _showTitle; }
            set { SetProperty(ref this._showTitle, value); }
        }

        private bool _isFirstItem;

        /// <summary>
        /// Indicates if this item is the first item in the navigation bar.
        /// </summary>
        public bool IsFirstItem
        {
            get { return _isFirstItem; }
            set { SetProperty(ref this._isFirstItem, value); }
        }

        private string _icon;

        /// <summary>
        /// The icon of the item.
        /// </summary>
        public string Icon
        {
            get { return _icon; }
            set { SetProperty(ref this._icon, value); }
        }
        /// <inheritdoc />
        public bool IsContentInitialized { get; private set; }

        /// <inheritdoc />
        public void InitializeContent()
        {
            var vm = _destination as ViewModelBase;
            IsContentInitialized = true;
            vm.Initialize().ContinueWith(t => ContentInitialized?.Invoke(this, EventArgs.Empty));
        }

        /// <inheritdoc />
        public virtual void Navigate()
        {
            NavigationRequested?.Invoke(this, _destination);
            if (_destination is ViewModelBase vm && !vm.IsInitialized && !IsContentInitialized)
            {
                InitializeContent();
            }
        }

        /// <inheritdoc />
        public virtual void SetDestination(IPresentable destination)
        {
            _destination = destination;
            DestinationResolved = true;
        }

        /// <inheritdoc />
        protected override Task OnInitialization()
        {
            if (_localizedTitle)
                Title = _localization.BindValue(this, e => e.Title, Title);
            if (IsSelected)
                Navigate();
            return base.OnInitialization();
        }
    }

    /// <inheritdoc />
    public class NavigationItem<TViewModel> : NavigationItem where TViewModel : IViewModel, IPresentable
    {
        /// <inheritdoc />
        public NavigationItem(string title,
            string icon,
            NavItemPosition position = NavItemPosition.Top,
            bool localizedTitle = false)
            : base(null, title, icon, position, localizedTitle)
        {
            DestinationType = typeof(TViewModel);
            DestinationResolved = false;
        }
    }
}