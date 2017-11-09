// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;
using System.Windows.Input;

namespace Coddee.WPF.XamlHelpers
{
    /// <summary>
    /// Sets focus to an element when the load event is triggered
    /// </summary>
    public class FocusOnLoad
    {
        public static readonly DependencyProperty ElementProperty =
            DependencyProperty.RegisterAttached("Element",
                                                typeof(FrameworkElement),
                                                typeof(FocusOnLoad),
                                                new PropertyMetadata(default(FrameworkElement), ValueSet));

        private static void ValueSet(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement el && e.NewValue is IInputElement inputElement)
            {
                el.FocusOnLoad(inputElement);
            }
        }
        
        public static void SetElement(DependencyObject element, IInputElement value)
        {
            element.SetValue(ElementProperty, value);
        }

        public static IInputElement GetElement(DependencyObject element)
        {
            return (IInputElement)element.GetValue(ElementProperty);
        }
    }
}