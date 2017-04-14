// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Coddee.WPF.Commands;

namespace Coddee.WPF.Controls
{
    [ContentProperty("Content")]
    [DefaultProperty("Content")]
    public class TabsPanelItem : Control, INotifyPropertyChanged
    {
        static TabsPanelItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabsPanelItem),
                                                     new FrameworkPropertyMetadata(typeof(TabsPanelItem)));
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
                                                                                                "Content",
                                                                                                typeof(object),
                                                                                                typeof(TabsPanelItem),
                                                                                                new
                                                                                                    PropertyMetadata(default
                                                                                                                     (
                                                                                                                         object
                                                                                                                     )))
            ;
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
                                                                                               "Header",
                                                                                               typeof(string),
                                                                                               typeof(TabsPanelItem),
                                                                                               new
                                                                                                   PropertyMetadata(default
                                                                                                                    (
                                                                                                                        string
                                                                                                                    )));

        public static readonly DependencyProperty PresentableProperty = DependencyProperty.Register(
                                                                                                    "Presentable",
                                                                                                    typeof(IPresentable
                                                                                                    ),
                                                                                                    typeof(TabsPanelItem
                                                                                                    ),
                                                                                                    new
                                                                                                        PropertyMetadata(default
                                                                                                                         (
                                                                                                                             IPresentable
                                                                                                                         ),
                                                                                                                         OnPresentableSet))
            ;

        private static void OnPresentableSet(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tablePanelItem = d as TabsPanelItem;
            if (tablePanelItem != null && e.NewValue is IPresentable)
                tablePanelItem.SetContentFromPresentable((IPresentable) e.NewValue);
        }

        private void SetContentFromPresentable(IPresentable presentable)
        {
            Content = presentable.GetView();
        }

        public IPresentable Presentable
        {
            get { return (IPresentable) GetValue(PresentableProperty); }
            set { SetValue(PresentableProperty, value); }
        }

        public string Header
        {
            get { return (string) GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }


        public object Content
        {
            get { return (object) GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }


        public int ID { get; set; }


        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                OnPropertyChanged();
            }
        }

        public Action<TabsPanelItem> ItemSelected = delegate { };
        public ICommand TabSelectedCommand => new RelayCommand(OnItemSelect);

        public void OnItemSelect()
        {
            ItemSelected?.Invoke(this);
            IsSelected = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [ContentProperty("Items")]
    [DefaultProperty("Items")]
    public class TabsPanel : Control, INotifyPropertyChanged
    {
        static TabsPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabsPanel),
                                                     new FrameworkPropertyMetadata(typeof(TabsPanel)));
        }

        public TabsPanel()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Items = new TabsPanelItemsCollection();
                Items.CollectionChanged += Items_CollectionChanged;
                Loaded += delegate
                {
                    if (double.IsNaN(MaxContentHeight))
                        MaxContentHeight = ActualHeight - TabsBarHeight - (ItemsMargin.Bottom + ItemsMargin.Top) - 5;
                };
            });
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items",
                                        typeof(TabsPanelItemsCollection),
                                        typeof(TabsPanel),
                                        new PropertyMetadata(default(TabsPanelItemsCollection)));
        public static readonly DependencyProperty ItemBackgroundProperty =
            DependencyProperty.Register("ItemBackground",
                                        typeof(SolidColorBrush),
                                        typeof(TabsPanel),
                                        new PropertyMetadata(default(SolidColorBrush)));

        public static readonly DependencyProperty ItemsMarginProperty =
            DependencyProperty.Register("ItemsMargin",
                                        typeof(Thickness),
                                        typeof(TabsPanel),
                                        new PropertyMetadata(default(Thickness)));

        public static readonly DependencyProperty TabsBarHeightProperty =
            DependencyProperty.Register("TabsBarHeight",
                                        typeof(double),
                                        typeof(TabsPanel),
                                        new PropertyMetadata(default(double)));

        public static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty.Register("HeaderForeground",
                                        typeof(SolidColorBrush),
                                        typeof(TabsPanel),
                                        new PropertyMetadata(default(SolidColorBrush)));
        public static readonly DependencyProperty MaxContentHeightProperty =
            DependencyProperty.Register("MaxContentHeight",
                                        typeof(double),
                                        typeof(TabsPanel),
                                        new PropertyMetadata(double.NaN));

        public double MaxContentHeight
        {
            get { return (double) GetValue(MaxContentHeightProperty); }
            set { SetValue(MaxContentHeightProperty, value); }
        }

        public SolidColorBrush HeaderForeground
        {
            get { return (SolidColorBrush) GetValue(HeaderForegroundProperty); }
            set { SetValue(HeaderForegroundProperty, value); }
        }

        public double TabsBarHeight
        {
            get { return (double) GetValue(TabsBarHeightProperty); }
            set { SetValue(TabsBarHeightProperty, value); }
        }
        public Thickness ItemsMargin
        {
            get { return (Thickness) GetValue(ItemsMarginProperty); }
            set { SetValue(ItemsMarginProperty, value); }
        }
        public SolidColorBrush ItemBackground
        {
            get { return (SolidColorBrush) GetValue(ItemBackgroundProperty); }
            set { SetValue(ItemBackgroundProperty, value); }
        }
        public TabsPanelItemsCollection Items
        {
            get { return (TabsPanelItemsCollection) GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        private TabsPanelItem _currentTab;
        public TabsPanelItem CurrentTab
        {
            get { return _currentTab; }
            set
            {
                _currentTab = value;
                OnPropertyChanged();
            }
        }


        private void OnItemSelected(TabsPanelItem obj)
        {
            foreach (var item in Items)
            {
                item.IsSelected = false;
            }
            this.CurrentTab = obj;
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    ((TabsPanelItem) item).ItemSelected += OnItemSelected;
                }
                if (CurrentTab == null && Items.Any())
                    Items[0].OnItemSelect();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TabsPanelItemsCollection : ObservableCollection<TabsPanelItem>
    {
    }
}