// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Coddee.WPF.Controls
{
    [DefaultProperty("Fields")]
    [ContentProperty("Fields")]
    [TemplatePart(Name = "PART_CONTAINER", Type = typeof(Panel))]
    public class Form : Control
    {
        static Form()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Form), new FrameworkPropertyMetadata(typeof(Form)));
        }
        public Form()
        {
            Fields = new FormFieldsCollection();
            Fields.CollectionChanged += Fields_CollectionChanged;
            LayoutUpdated += Form_LayoutUpdated;
            GotFocus += Form_GotFocus;
        }

        private void Form_GotFocus(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
            if (Fields.Any())
            {
                var first = Fields.FirstOrDefault();
                if (first != null)
                {
                    if (first.Content is FrameworkElement elem)
                    {
                        elem.Focus();
                    }
                    if (first.Content is IInputElement input)
                    {
                        Keyboard.Focus(input);
                    }
                }
            }
        }

        public static readonly DependencyProperty FieldsProperty = DependencyProperty.Register(
            "Fields", typeof(FormFieldsCollection), typeof(Form), new PropertyMetadata(new FormFieldsCollection()/*, FieldsSet*/));

        public static readonly DependencyProperty FieldsMarginProperty = DependencyProperty.Register(
            "FieldsMargin", typeof(Thickness), typeof(Form), new PropertyMetadata(default(Thickness)));


        private double _titlesWidth;
        public Thickness FieldsMargin
        {
            get { return (Thickness)GetValue(FieldsMarginProperty); }
            set { SetValue(FieldsMarginProperty, value); }
        }


        public FormFieldsCollection Fields
        {
            get { return (FormFieldsCollection)GetValue(FieldsProperty); }
            set { SetValue(FieldsProperty, value); }
        }

        private void Fields_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (FormField item in e.NewItems)
                {
                    if (item.Margin == new Thickness(0, 0, 0, 0))
                        item.Margin = FieldsMargin;
                    this.CalculateWidth();
                }
            }
        }

        void CalculateWidth()
        {
            var labels = FindVisualChildren<Label>(this);
            if (labels.Any())
            {
                var max = labels.Max(e => e.ActualWidth);
                if (max > _titlesWidth)
                {
                    foreach (var label in labels)
                    {
                        label.SetValue(FrameworkElement.WidthProperty, max);
                    }
                    _titlesWidth = max;
                }
            }
        }

        private void Form_LayoutUpdated(object sender, EventArgs e)
        {
            this.CalculateWidth();
        }
        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {

                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }

    public class FormFieldsCollection : ObservableCollection<FormField>
    {

    }
}
