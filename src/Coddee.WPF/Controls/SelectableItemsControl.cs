using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using Coddee.Collections;
using System.Linq;

namespace Coddee.WPF.Controls
{

    /// <summary>
    /// A selectable object wrapper.
    /// </summary>
    public class SelectableItem : SelectableItem<object> {

        /// <inheritdoc />
        public SelectableItem(object item, bool isSelected = false) : base(item, isSelected)
        {
        }
    }

    /// <summary>
    /// An extended <see cref="ItemsControl"/> control that provide selection.
    /// </summary>
    public class SelectableItemsControl : ItemsControl
    {
        static SelectableItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectableItemsControl), new FrameworkPropertyMetadata(typeof(SelectableItemsControl)));
        }

        /// <summary>
        /// DataTemplate for selected items.
        /// </summary>
        public static readonly DependencyProperty SelectedItemTemplateProperty = DependencyProperty.Register(
                                                        "SelectedItemTemplate",
                                                        typeof(DataTemplate),
                                                        typeof(SelectableItemsControl),
                                                        new PropertyMetadata(default(DataTemplate)));

        /// <summary>
        /// The currently selected item
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
                                                        "SelectedItem",
                                                        typeof(object),
                                                        typeof(SelectableItemsControl),
                                                        new FrameworkPropertyMetadata(default(object), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// The items wrapped in selectable objects.
        /// </summary>
        public static readonly DependencyPropertyKey SelectablesPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "Selectables",
                                                                      typeof(AsyncObservableCollection<SelectableItem>),
                                                                      typeof(SelectableItemsControl),
                                                                      new PropertyMetadata(default(AsyncObservableCollection<SelectableItem>)));

        ///<inheritdoc cref="SelectablesPropertyKey"/>
        public static readonly DependencyProperty SelectablesProperty = SelectablesPropertyKey.DependencyProperty;

        ///<inheritdoc cref="SelectablesPropertyKey"/>
        public AsyncObservableCollection<SelectableItem> Selectables
        {
            get { return (AsyncObservableCollection<SelectableItem>)GetValue(SelectablesProperty); }
            protected set { SetValue(SelectablesPropertyKey, value); }
        }

        ///<inheritdoc cref="SelectedItemProperty"/>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set
            {

                SetValue(SelectedItemProperty, value);
                if (_lastSelected.Item != value)
                {
                    var item = Selectables.FirstOrDefault(e => e.Item.Equals(value));
                    item?.Select();
                }
            }
        }

        ///<inheritdoc cref="SelectedItemTemplateProperty"/>
        public DataTemplate SelectedItemTemplate
        {
            get { return (DataTemplate)GetValue(SelectedItemTemplateProperty); }
            set { SetValue(SelectedItemTemplateProperty, value); }
        }



        private SelectableItem _lastSelected;

        /// <inheritdoc />
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            Selectables = AsyncObservableCollection<SelectableItem>.Create();
            foreach (var newItem in newValue)
            {
                Selectables.Add(CreateSelectable(newItem));
            }

            if (newValue is INotifyCollectionChanged notify)
                notify.CollectionChanged += OnCollectionChanged;

            if (oldValue is INotifyCollectionChanged oldNotify)
                oldNotify.CollectionChanged -= OnCollectionChanged;
        }

        private SelectableItem CreateSelectable(object newItem)
        {
            var selectableItem = new SelectableItem(newItem);
            selectableItem.Selected += ItemSelected;
            return selectableItem;
        }

        private void ItemSelected(object sender, object e)
        {
            if (_lastSelected != null)
                _lastSelected.IsSelected = false;

            _lastSelected = (SelectableItem)sender;

            if (SelectedItem != e)
                SelectedItem = e;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var newItem in args.NewItems)
                    {
                        Selectables.Add(CreateSelectable(newItem));
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var newItem in args.NewItems)
                    {
                        Selectables.Remove(e => e.Item.Equals(newItem));
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Selectables.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
