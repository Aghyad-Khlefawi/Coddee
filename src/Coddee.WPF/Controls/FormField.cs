// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Coddee.WPF.Controls
{
    /// <summary>
    /// A form field for <see cref="Form"/> control.
    /// </summary>
    [DefaultProperty("Content")]
    [ContentProperty("Content")]
    public class FormField : Control
    {
        static FormField()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormField), new FrameworkPropertyMetadata(typeof(FormField)));
            VisibilityProperty.OverrideMetadata(typeof(FormField), new PropertyMetadata(System.Windows.Visibility.Visible, OnVisiblityChanged));
            ValidatedPropertyNameProperty = ValidationBorder.ValidatedPropertyNameProperty.AddOwner(typeof(FormField));
        }

        /// <summary>
        /// The validation property name.
        /// </summary>
        public static readonly DependencyProperty ValidatedPropertyNameProperty;

        /// <summary>
        /// The title of the field.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(FormField), new PropertyMetadata(default(string)));

        /// <summary>
        /// The width of the title.
        /// </summary>
        public static readonly DependencyProperty TitleWidthProperty = DependencyProperty.Register(
            "TitleWidth", typeof(double), typeof(FormField), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Shows a busy indicators if true.
        /// </summary>
        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
            "IsBusy", typeof(bool), typeof(FormField), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The content of the field.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(object), typeof(FormField), new PropertyMetadata(default(object), ContentPropertyChanged));

        private static void ContentPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is FormField formField)
                formField.OnContentPropertyChanged();

        }

        /// <summary>
        /// The style of the title.
        /// </summary>
        public static readonly DependencyProperty TitleStyleProperty = DependencyProperty.Register(
                                                        "TitleStyle",
                                                        typeof(Style),
                                                        typeof(FormField),
                                                        new PropertyMetadata(default(Style)));

        /// <summary>
        /// If true a <see cref="ValidationBorder"/> will be attached.
        /// </summary>
        public static readonly DependencyProperty ShowValidationBorderProperty = DependencyProperty.Register(
                                                        "ShowValidationBorder",
                                                        typeof(bool?),
                                                        typeof(FormField),
                                                        new PropertyMetadata(null));
        /// <inheritdoc cref="ShowValidationBorderProperty"/>
        public bool? ShowValidationBorder
        {
            get { return (bool?)GetValue(ShowValidationBorderProperty); }
            set { SetValue(ShowValidationBorderProperty, value); }
        }
        /// <inheritdoc cref="ValidatedPropertyNameProperty"/>
        public string ValidatedPropertyName
        {
            get { return (string)GetValue(ValidatedPropertyNameProperty); }
            set { SetValue(ValidatedPropertyNameProperty, value); }
        }

        /// <inheritdoc cref="TitleStyleProperty"/>
        public Style TitleStyle
        {
            get { return (Style)GetValue(TitleStyleProperty); }
            set { SetValue(TitleStyleProperty, value); }
        }
        /// <inheritdoc cref="ContentProperty"/>
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }


        /// <inheritdoc cref="IsBusyProperty"/>
        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        /// <inheritdoc cref="TitleWidthProperty"/>
        public double TitleWidth
        {
            get { return (double)GetValue(TitleWidthProperty); }
            set { SetValue(TitleWidthProperty, value); }
        }

        /// <inheritdoc cref="TitleProperty"/>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        private Label _titleControl;

        /// <summary>
        /// Triggered when the title label is set.
        /// </summary>
        public event Action TitleControlSet;

        /// <summary>
        /// Triggered when the visibility of the control is changed.
        /// </summary>
        public event Action VisbilityChanged;

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _titleControl = (Label)GetTemplateChild("PART_TITLE");
            _titleControl.SizeChanged += (s, e) => TitleControlSet?.Invoke();

            if (string.IsNullOrWhiteSpace(ValidatedPropertyName) && Content is Control control)
            {
                Dispatcher.Invoke(() =>
                {
                    var val = Validate.GetFieldName(control);
                    if (!string.IsNullOrWhiteSpace(val))
                        ValidatedPropertyName = val;
                });
            }
        }

        private static void OnVisiblityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FormField field)
                field.VisbilityChanged?.Invoke();
        }

        private void OnContentPropertyChanged()
        {
            if(ShowValidationBorder==null)
                ShowValidationBorder = !(Content is Label) && !(Content is TextBlock) && !(Content is Panel);
        }
    }

}
