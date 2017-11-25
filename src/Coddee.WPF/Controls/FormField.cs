// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Coddee.WPF.Controls
{
    [DefaultProperty("Content")]
    [ContentProperty("Content")]
    public class FormField : Control
    {
        static FormField()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormField), new FrameworkPropertyMetadata(typeof(FormField)));
            VisibilityProperty.OverrideMetadata(typeof(FormField), new PropertyMetadata(System.Windows.Visibility.Visible, OnVisiblityChanged));
        }


        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(FormField), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty TitleWidthProperty = DependencyProperty.Register(
            "TitleWidth", typeof(double), typeof(FormField), new PropertyMetadata(double.NaN));

        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
            "IsBusy", typeof(bool), typeof(FormField), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(object), typeof(FormField), new PropertyMetadata(default(object)));
        public static readonly DependencyProperty TitleStyleProperty = DependencyProperty.Register(
                                                        "TitleStyle",
                                                        typeof(Style),
                                                        typeof(FormField),
                                                        new PropertyMetadata(default(Style)));

        public Style TitleStyle
        {
            get { return (Style)GetValue(TitleStyleProperty); }
            set { SetValue(TitleStyleProperty, value); }
        }
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        public double TitleWidth
        {
            get { return (double)GetValue(TitleWidthProperty); }
            set { SetValue(TitleWidthProperty, value); }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public Label TitleControl { get; set; }
        public event Action TitleControlSet;
        public event Action VisbilityChanged;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TitleControl = (Label)GetTemplateChild("PART_TITLE");
            TitleControl.SizeChanged += (s, e) => TitleControlSet?.Invoke();
        }


        private static void OnVisiblityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FormField field)
                field.VisbilityChanged?.Invoke();
        }
    }

}
