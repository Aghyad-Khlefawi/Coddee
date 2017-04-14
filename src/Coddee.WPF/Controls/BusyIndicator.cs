// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Coddee.WPF.Controls
{
    [DefaultProperty("Content")]
    [ContentProperty("Content")]
    public class BusyIndicator : ContentControl
    {
        static BusyIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BusyIndicator), new FrameworkPropertyMetadata(typeof(BusyIndicator)));
        }

        public BusyIndicator()
        {
            Background = new SolidColorBrush(Colors.WhiteSmoke);
        }

        public static readonly DependencyProperty ContentItemProperty = DependencyProperty.Register("ContentItem", typeof(object), typeof(BusyIndicator));
        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool),
            typeof(BusyIndicator), new PropertyMetadata(false, BusyStatusChange));

        public static readonly DependencyProperty FillColorProperty = DependencyProperty.Register("FillColor", typeof(SolidColorBrush),
            typeof(BusyIndicator), new PropertyMetadata(new SolidColorBrush(Colors.LightBlue)));

        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double),
            typeof(BusyIndicator), new PropertyMetadata(double.Parse("10")));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string),
            typeof(BusyIndicator), new PropertyMetadata("Loading ..."));

        public static readonly DependencyProperty HideTextProperty = DependencyProperty.Register(
            "HideText", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty HideContentOnBusyProperty = DependencyProperty.Register(
            "HideContentOnBusy", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(default(bool)));

        public bool HideContentOnBusy
        {
            get { return (bool)GetValue(HideContentOnBusyProperty); }
            set { SetValue(HideContentOnBusyProperty, value); }
        }

        public bool HideText
        {
            get { return (bool)GetValue(HideTextProperty); }
            set { SetValue(HideTextProperty, value); }
        }

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }
        public object ContentItem
        {
            get
            {
                return this.GetValue(ContentItemProperty);
            }
            set
            {
                this.SetValue(ContentItemProperty, value);
            }
        }
        public SolidColorBrush FillColor
        {
            get { return (SolidColorBrush)GetValue(FillColorProperty); }
            set { SetValue(FillColorProperty, value); }
        }
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public event Action BusyChange = delegate { };

        private static void BusyStatusChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BusyIndicator).BusyChange();
        }
    }

}
