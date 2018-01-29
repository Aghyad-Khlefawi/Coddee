// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Coddee.Validation;

namespace Coddee.WPF.Controls
{
    [DefaultProperty(nameof(ValidationBorder.Child))]
    [ContentProperty(nameof(ValidationBorder.Child))]
    public class ValidationBorder : Control
    {
        public static SolidColorBrush ErrorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D91D1D"));
        public static SolidColorBrush WarningBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF39524"));
        public static SolidColorBrush GrayBrush = SystemColors.ActiveBorderBrush;

        static ValidationBorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ValidationBorder), new FrameworkPropertyMetadata(typeof(ValidationBorder)));
            DataContextProperty.OverrideMetadata(typeof(ValidationBorder), new FrameworkPropertyMetadata(default(object), FrameworkPropertyMetadataOptions.Inherits, BorderDataContextChanged));
        }



        public static readonly DependencyProperty ValidatedPropertyNameProperty = DependencyProperty.Register(
                                                        "ValidatedPropertyName",
                                                        typeof(string),
                                                        typeof(ValidationBorder),
                                                        new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.Inherits, ValidatedPropertyNamePropertyChanged));

        public static readonly DependencyProperty ChildProperty = DependencyProperty.Register(
                                                        "Child",
                                                        typeof(Control),
                                                        typeof(ValidationBorder),
                                                        new FrameworkPropertyMetadata(default(Control), ChildPropertyChanged));
        public static readonly DependencyPropertyKey HasWarningPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "HasWarning",
                                                                      typeof(bool),
                                                                      typeof(ValidationBorder),
                                                                      new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty HasWarningProperty = HasWarningPropertyKey.DependencyProperty;
        public static readonly DependencyPropertyKey HasErrorPropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "HasError",
                                                                      typeof(bool),
                                                                      typeof(ValidationBorder),
                                                                      new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty HasErrorProperty = HasErrorPropertyKey.DependencyProperty;

        public static readonly DependencyPropertyKey MessagePropertyKey = DependencyProperty.RegisterReadOnly(
                                                                      "Message",
                                                                      typeof(string),
                                                                      typeof(ValidationBorder),
                                                                      new PropertyMetadata(default(string)));

        public static readonly DependencyProperty MessageProperty = MessagePropertyKey.DependencyProperty;
       

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            protected set { SetValue(MessagePropertyKey, value); }
        }

        public bool HasError
        {
            get { return (bool)GetValue(HasErrorProperty); }
            protected set { SetValue(HasErrorPropertyKey, value); }
        }
        public bool HasWarning
        {
            get { return (bool)GetValue(HasWarningProperty); }
            protected set { SetValue(HasWarningPropertyKey, value); }
        }

        public Control Child
        {
            get { return (Control)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }

        public string ValidatedPropertyName
        {
            get { return (string)GetValue(ValidatedPropertyNameProperty); }
            set { SetValue(ValidatedPropertyNameProperty, value); }
        }

        private static void ValidatedPropertyNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ValidationBorder border)
                border.OnValidatedPropertyNamePropertyChanged();
        }

        private static void ChildPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is ValidationBorder border)
                border.OnChildChanged();
        }

        private static void BorderDataContextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is ValidationBorder border)
                border.OnBorderDataContextChanged();
        }

        private IEnumerable<IValidationRule> _validationRules;

        private void OnBorderDataContextChanged()
        {
            RefreshValidationRules();
        }

        private void OnValidatedPropertyNamePropertyChanged()
        {
            RefreshValidationRules();
        }

        private void RefreshValidationRules()
        {
            Dispatcher.Invoke(() =>
            {
                if (DataContext is IViewModel vm)
                {
                    GetValidationRules(vm);
                    vm.PropertyChanged += (sender, args) =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if (args.PropertyName == ValidatedPropertyName)
                                Validate();
                        });
                    };
                    vm.ValidationRulesSet += delegate
                    {
                        GetValidationRules(vm);
                    };
                }
            });
        }

        private void OnChildChanged()
        {
            if (Child != null && Child.BorderThickness != _zeroThickness)
                Child.BorderThickness = _zeroThickness;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Child != null && Child.BorderThickness != _zeroThickness)
                Child.BorderThickness = _zeroThickness;
        }

        private void GetValidationRules(IViewModel vm)
        {
            _validationRules = vm.ValidationRules.Where(e => e.FieldName == ValidatedPropertyName).OrderBy(e => e.ValidationType);
            Validate();
        }

        private void Validate()
        {
            Dispatcher.Invoke(() =>
            {
                foreach (var rule in _validationRules)
                {
                    if (!rule.Validate())
                    {
                        BorderBrush = rule.ValidationType == ValidationType.Error ? ErrorBrush : WarningBrush;
                        HasError = rule.ValidationType == ValidationType.Error;
                        HasWarning = !HasError;
                        Message = rule.GetMessage();
                        return;
                    }
                }
                HasError = HasWarning = false;
                BorderBrush = GrayBrush;
            });
        }

        private readonly Thickness _zeroThickness = new Thickness(0);


    }
}
