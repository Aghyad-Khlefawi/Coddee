// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Collections.Generic;
using System.Windows;

namespace Coddee.WPF.XamlHelpers
{

    public class VisibilityConditions
    {
        public static readonly DependencyProperty VisiblityPropertyProperty = DependencyProperty.RegisterAttached(
                                                                "VisiblityProperty",
                                                                typeof(object),
                                                                typeof(VisibilityConditions),
                                                                new PropertyMetadata(default(object), OnVisiblityPropertyChanged));

        public static readonly DependencyProperty VisibleValuesProperty = DependencyProperty.RegisterAttached(
                                                                "VisibleValues",
                                                                typeof(VisibleValuesCollection),
                                                                typeof(VisibilityConditions),
                                                                new PropertyMetadata(new VisibleValuesCollection()));



        public static void SetVisibleValues(DependencyObject element, VisibleValuesCollection value)
        {
            element.SetValue(VisibleValuesProperty, value);
        }

        public static VisibleValuesCollection GetVisibleValues(DependencyObject element)
        {
            return (VisibleValuesCollection)element.GetValue(VisibleValuesProperty);
        }

        public static void SetVisiblityProperty(DependencyObject element, object value)
        {
            element.SetValue(VisiblityPropertyProperty, value);
        }

        public static string GetVisiblityProperty(DependencyObject element)
        {
            return (string)element.GetValue(VisiblityPropertyProperty);
        }

        private static void OnVisiblityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                if (element.GetValue(VisibleValuesProperty) is VisibleValuesCollection visiblityValues)
                {
                    element.Visibility = visiblityValues.Contains(e.NewValue)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }
        }
    }

    public class VisibleValuesCollection : List<object> { }
}
