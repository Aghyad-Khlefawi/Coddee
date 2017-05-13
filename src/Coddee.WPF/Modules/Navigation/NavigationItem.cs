// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Coddee.WPF.Commands;

namespace Coddee.WPF.Navigation
{
    public enum NavItemPosition
    {
        Top,
        Bottom
    }

    public interface INavigationItem
    {
        Type DestinationType { get;  }
        bool DestinationResolved { get; }
        bool IsSelected { get; set; }
        bool ShowTitle { get; set; }
        bool IsInitialized { get; }
        bool IsVisible { get; set; }

        Task Initialize();
        void SetDestination(IPresentable destination);
        event EventHandler<IPresentable> NavigationRequested;
    }

    public class NavigationItem : ViewModelBase, INavigationItem
    {
        public NavigationItem(
            IPresentable destination,
            string title,
            string icon,
            NavItemPosition position = NavItemPosition.Top)
        {
            _destination = destination;
            Title = title;
            if (!string.IsNullOrEmpty(icon))
                Icon = Geometry.Parse(icon);
            Position = position;
            DestinationType = _destination?.GetType();
            DestinationResolved = true;
        }

        private bool _isVisible=true;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref this._isVisible, value); }
        }

        public Type DestinationType { get; protected set; }
        public bool DestinationResolved { get; protected set; }

        public event EventHandler<IPresentable> NavigationRequested;

        protected IPresentable _destination;

        private NavItemPosition _position;
        public NavItemPosition Position
        {
            get { return _position; }
            set
            {
                SetProperty(ref this._position, value);
                Dock = Position == NavItemPosition.Bottom ? Dock.Bottom : Dock.Top;
            }
        }

        private Dock _dock;
        public Dock Dock
        {
            get { return _dock; }
            set { SetProperty(ref this._dock, value); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref this._isSelected, value); }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref this._title, value); }
        }

        private bool _showTitle;
        public bool ShowTitle
        {
            get { return _showTitle; }
            set { SetProperty(ref this._showTitle, value); }
        }

        private bool _isFirstItem;
        public bool IsFirstItem
        {
            get { return _isFirstItem; }
            set { SetProperty(ref this._isFirstItem, value); }
        }
        private Geometry _icon;
        public Geometry Icon
        {
            get { return _icon; }
            set { SetProperty(ref this._icon, value); }
        }
        public ICommand NavigateCommand => new RelayCommand(Navigate);

        protected virtual void Navigate()
        {
            NavigationRequested?.Invoke(this, _destination);
        }

        public virtual void SetDestination(IPresentable destination)
        {
            _destination = destination;
            DestinationResolved = true;
        }

        protected override async Task OnInitialization()
        {
            var vm = _destination as ViewModelBase;
            if (vm != null)
            {
                await vm.Initialize();
            }
        }
    }

    public class NavigationItem<TViewModel> : NavigationItem where TViewModel : IViewModel, IPresentable
    {
        public NavigationItem(string title,
                              string icon,
                              NavItemPosition position = NavItemPosition.Top)
            : base(null, title, icon, position)
        {
            DestinationType = typeof(TViewModel);
            DestinationResolved = false;
        }
    }
}