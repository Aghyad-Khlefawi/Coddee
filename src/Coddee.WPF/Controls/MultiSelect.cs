// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Coddee.Collections;

namespace Coddee.WPF.Controls
{
    [TemplatePart(Name = PART_RootBorder, Type = typeof(Border))]
    [TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
    [TemplatePart(Name = PART_OpenButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_AllButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_AllNone, Type = typeof(ButtonBase))]
    public class MultiSelect : CoddeeControl
    {
        private const string PART_RootBorder = nameof(PART_RootBorder);
        private const string PART_OpenButton = nameof(PART_OpenButton);
        private const string PART_Popup = nameof(PART_Popup);
        private const string PART_AllButton = nameof(PART_AllButton);
        private const string PART_AllNone = nameof(PART_AllNone);

        static MultiSelect()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiSelect), new FrameworkPropertyMetadata(typeof(MultiSelect)));
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
                                                        "ItemsSource",
                                                        typeof(object),
                                                        typeof(MultiSelect),
                                                        new PropertyMetadata(default(object), OnItemsSourcePropertyChanged));



        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
                                                        "SelectedItems",
                                                        typeof(object),
                                                        typeof(MultiSelect),
                                                        new PropertyMetadata(default(object)));

        public static readonly DependencyProperty DisplayMemberPathProperty = DependencyProperty.Register(
                                                        "DisplayMemberPath",
                                                        typeof(string),
                                                        typeof(MultiSelect),
                                                        new PropertyMetadata(default(string)));
        public static readonly DependencyPropertyKey ContentPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "Content",
                                                                      typeof(string),
                                                                      typeof(MultiSelect),
                                                                      new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ContentProperty = ContentPropertyKey.DependencyProperty;

        public string Content
        {
            get { return (string)GetValue(ContentProperty); }
            protected set { SetValue(ContentPropertyKey, value); }
        }

        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiSelect multiSelect && e.NewValue is IList enumerable)
            {
                multiSelect.OnItemSourceChanged(enumerable);

            }
        }

        PropertyInfo displatProperty;

        private void OnItemSourceChanged(IList enumerable)
        {
            Selectables = new AsyncObservableCollection<SelectableItem<object>>(enumerable.Count);
            foreach (var item in enumerable)
            {
                if (displatProperty == null)
                    displatProperty = item.GetType().GetProperty(DisplayMemberPath);

                if (displatProperty != null)
                {
                    var selectable = SelectableItem<object>.Create(item);
                    selectable.Title = GetTitle(item);
                    selectable.SelectChanged += SelectableSelectChanged;
                    Selectables.Add(selectable);
                }
            }
        }

        private string GetTitle(object item)
        {
            return (string)displatProperty.GetValue(item);
        }


        public object SelectedItems
        {
            get { return (object)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private AsyncObservableCollection<SelectableItem<object>> _selectables;
        public AsyncObservableCollection<SelectableItem<object>> Selectables
        {
            get { return _selectables; }
            set
            {
                _selectables = value;
                OnPropertyChanged();
            }
        }

        private void SelectableSelectChanged(object sender, object arg)
        {
            if (_supressEvents)
                return;

            if (SelectedItems is IAsyncObservableCollection collection)
            {
                collection.Clear();
                var selected = Selectables.Where(e => e.IsSelected).Select(e => e.Item);
                collection.AddRangeObject(selected);
                Content = selected.Select(GetTitle).Combine(",");
            }
        }


        private Border _partRootBorder;
        private ButtonBase _partOpenButton;
        private ButtonBase _partAllButton;
        private ButtonBase _partNoneButton;
        private Popup _partPopup;

        private bool _supressEvents;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _partRootBorder = (Border)GetTemplateChild(PART_RootBorder);
            _partOpenButton = (ButtonBase)GetTemplateChild(PART_OpenButton);
            _partAllButton = (ButtonBase)GetTemplateChild(PART_AllButton);
            _partNoneButton = (ButtonBase)GetTemplateChild(PART_AllNone);
            _partPopup = (Popup)GetTemplateChild(PART_Popup);

            if (_partOpenButton != null)
                _partOpenButton.Click += OpenButtonClicked;

            if (_partAllButton != null)
                _partAllButton.Click += SelectAllButtonClicked;

            if (_partNoneButton != null)
                _partNoneButton.Click += SelectNoneButtonClicked;
        }
        private void SelectNoneButtonClicked(object sender, RoutedEventArgs arg)
        {
            _supressEvents = true;
            Selectables.ForEach(e => e.IsSelected = false);
            _supressEvents = false;
            SelectableSelectChanged(null,null);
        }



        private void SelectAllButtonClicked(object sender, RoutedEventArgs arg)
        {
            _supressEvents = true;
            Selectables.ForEach(e => e.IsSelected = true);
            _supressEvents = false;
            SelectableSelectChanged(null, null);
        }

        private void OpenButtonClicked(object sender, RoutedEventArgs e)
        {
            if (_partPopup != null)
            {
                _partPopup.IsOpen = true;
            }
        }
    }
}
