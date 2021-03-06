﻿// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Coddee.WPF.Controls
{
    /// <summary>
    /// A data form.
    /// </summary>
    [DefaultProperty("Fields")]
    [ContentProperty("Fields")]
    [TemplatePart(Name = "PART_CONTAINER", Type = typeof(Panel))]
    public class Form : Control
    {
        static Form()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Form), new FrameworkPropertyMetadata(typeof(Form)));
        }

        /// <inheritdoc />
        public Form()
        {
            Fields = new FormFieldsCollection();
            Fields.CollectionChanged += Fields_CollectionChanged;
            LayoutUpdated += Form_LayoutUpdated;
            Focusable = false;
        }


        /// <summary>
        /// The form fields.
        /// </summary>
        public static readonly DependencyProperty FieldsProperty = DependencyProperty.Register("Fields", typeof(FormFieldsCollection), typeof(Form), new PropertyMetadata(new FormFieldsCollection(), FieldsSet));

        /// <summary>
        /// The margin between the fields
        /// </summary>
        public static readonly DependencyProperty FieldsMarginProperty = DependencyProperty.Register(
                                                                                                     "FieldsMargin", typeof(Thickness), typeof(Form), new PropertyMetadata(new Thickness(0, 0, 0, 3)));
        /// <summary>
        /// The style of the titles.
        /// </summary>
        public static readonly DependencyProperty TitleStyleProperty = DependencyProperty.Register(
                                                        "TitleStyle",
                                                        typeof(Style),
                                                        typeof(Form),
                                                        new PropertyMetadata(default(Style)));

        /// <inheritdoc cref="TitleStyleProperty"/>
        public Style TitleStyle
        {
            get { return (Style)GetValue(TitleStyleProperty); }
            set { SetValue(TitleStyleProperty, value); }
        }

        private double _titlesWidth;
        /// <inheritdoc cref="FieldsMarginProperty"/>
        public Thickness FieldsMargin
        {
            get { return (Thickness)GetValue(FieldsMarginProperty); }
            set { SetValue(FieldsMarginProperty, value); }
        }


        /// <inheritdoc cref="FieldsProperty"/>
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
                    ConfigureField(item);
                }
            }
        }

        private void ConfigureField(FormField item)
        {
            if (item.Margin == new Thickness(0, 0, 0, 0))
                item.Margin = FieldsMargin;

            if (TitleStyle != null)
                item.TitleStyle = TitleStyle;

            item.Loaded += delegate
            { CalculateWidth(); };
            item.LayoutUpdated += delegate
            { CalculateWidth(); };
            CalculateWidth();
        }

        private static void FieldsSet(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is FormFieldsCollection collection)
            {
                if (d is Form form)
                    collection.ForEach(form.ConfigureField);
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

    /// <summary>
    /// A collection of <see cref="FormField"/> objects.
    /// </summary>
    public class FormFieldsCollection : ObservableCollection<FormField>
    {

    }
}
