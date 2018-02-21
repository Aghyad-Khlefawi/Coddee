// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Coddee.WPF.Controls
{
    /// <summary>
    /// A control used to indicates that an element is busy and can't be interacted with.
    /// </summary>
    [DefaultProperty("Content")]
    [ContentProperty("Content")]
    public class BusyIndicator : ContentControl
    {
        static BusyIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BusyIndicator), new FrameworkPropertyMetadata(typeof(BusyIndicator)));
        }

        /// <inheritdoc />
        public BusyIndicator()
        {
            Background = new SolidColorBrush(Colors.WhiteSmoke);
            Focusable = false;
        }

        /// <summary>
        /// The content wrapped by the BusyIndicator
        /// </summary>
        public static readonly DependencyProperty ContentItemProperty = DependencyProperty.Register("ContentItem", typeof(object), typeof(BusyIndicator));

        /// <summary>
        /// When true the BusyIndicator will be visible.
        /// </summary>
        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool),
            typeof(BusyIndicator), new PropertyMetadata(false));

        /// <summary>
        /// The color of the BusyIndicator
        /// </summary>
        public static readonly DependencyProperty FillColorProperty = DependencyProperty.Register("FillColor", typeof(SolidColorBrush),
            typeof(BusyIndicator), new PropertyMetadata(new SolidColorBrush(Colors.LightBlue)));

        /// <summary>
        /// The radius used in the BusyIndicator ellipses
        /// </summary>
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double),
            typeof(BusyIndicator), new PropertyMetadata(double.Parse("10")));

        /// <summary>
        /// The text displayed in the BusyIndicator
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string),
            typeof(BusyIndicator), new PropertyMetadata("Loading ..."));

        /// <summary>
        /// If set to true the text will no be hidden when the BusyIndicator is displayed.
        /// </summary>
        public static readonly DependencyProperty HideTextProperty = DependencyProperty.Register(
            "HideText", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(default(bool)));

        /// <summary>
        /// If true the content of the BusyIndicator will be hidden when the BusyIndicator is displayed.
        /// </summary>
        public static readonly DependencyProperty HideContentOnBusyProperty = DependencyProperty.Register(
            "HideContentOnBusy", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(default(bool)));

        ///<inheritdoc cref="HideContentOnBusyProperty"/>
        public bool HideContentOnBusy
        {
            get { return (bool)GetValue(HideContentOnBusyProperty); }
            set { SetValue(HideContentOnBusyProperty, value); }
        }

        ///<inheritdoc cref="HideContentOnBusyProperty"/>
        public bool HideText
        {
            get { return (bool)GetValue(HideTextProperty); }
            set { SetValue(HideTextProperty, value); }
        }

        ///<inheritdoc cref="IsBusyProperty"/>
        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        ///<inheritdoc cref="ContentItemProperty"/>
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
        ///<inheritdoc cref="FillColorProperty"/>
        public SolidColorBrush FillColor
        {
            get { return (SolidColorBrush)GetValue(FillColorProperty); }
            set { SetValue(FillColorProperty, value); }
        }
        ///<inheritdoc cref="RadiusProperty"/>
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }
        ///<inheritdoc cref="TextProperty"/>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}
