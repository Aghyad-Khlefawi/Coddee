// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;
using System.Windows.Input;

namespace Coddee.WPF.XamlHelpers
{
    public class RelayScroll
    {
        public static readonly DependencyProperty ElementProperty = DependencyProperty.RegisterAttached(
                                                                "Element",
                                                                typeof(FrameworkElement),
                                                                typeof(RelayScroll),
                                                                new PropertyMetadata(default(FrameworkElement), ElementPropertyChanged));

        private static void ElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement source && e.NewValue is FrameworkElement element)
            {
                source.PreviewMouseWheel += (sender, args) =>
                {
                    args.Handled = true;
                    var ev = new MouseWheelEventArgs(args.MouseDevice, args.Timestamp, args.Delta)
                    {
                        RoutedEvent = UIElement.MouseWheelEvent
                    };
                    element.RaiseEvent(ev);
                };
            }
        }

        public static void SetElement(DependencyObject element, FrameworkElement value)
        {
            element.SetValue(ElementProperty, value);
        }

        public static FrameworkElement GetElement(DependencyObject element)
        {
            return (FrameworkElement)element.GetValue(ElementProperty);
        }

    }

}
