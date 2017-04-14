// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Coddee.WPF.Controls
{
    public class GlowBorder : Control
    {
        static GlowBorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GlowBorder),
                                                     new FrameworkPropertyMetadata(typeof(GlowBorder)));
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color",typeof(Color),typeof(GlowBorder),new PropertyMetadata(default(Color),OnColorSet));


        public static readonly DependencyProperty BrushProperty = DependencyProperty.Register("Brush",typeof(SolidColorBrush),typeof(GlowBorder),new PropertyMetadata(default(SolidColorBrush),OnBrushSet));

     

        public SolidColorBrush Brush
        {
            get { return (SolidColorBrush) GetValue(BrushProperty); }
            set { SetValue(BrushProperty, value); }
        }

        public Color Color
        {
            get { return (Color) GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }


        private static void OnColorSet(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var border = ((GlowBorder) d);
            var newColor = (Color) e.NewValue;
            if (border.Brush == null || border.Brush.Color != newColor)
                border.SetValue(BrushProperty, new SolidColorBrush(newColor));

        }

        private static void OnBrushSet(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var border = ((GlowBorder)d);
            var newBrush = (SolidColorBrush)e.NewValue;
            if(border.Color != newBrush.Color)
                border.SetValue(ColorProperty,newBrush.Color);
        }
    }
}