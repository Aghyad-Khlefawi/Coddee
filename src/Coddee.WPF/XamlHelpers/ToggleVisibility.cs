using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Coddee.WPF.XamlHelpers
{
    /// <summary>
    /// Attached to a button to toggle the visibility of a UIElement
    /// </summary>
    public class ToggleVisibility : DependencyObject
    {
        /// <summary>
        /// The element that visibility will be changed.
        /// </summary>
        public static readonly DependencyProperty ElementProperty = DependencyProperty.RegisterAttached(
                                                                "Element",
                                                                typeof(UIElement),
                                                                typeof(ToggleVisibility),
                                                                new PropertyMetadata(default(UIElement), OnElementSet));

        private static void OnElementSet(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ButtonBase button)
            {
                var element = e.NewValue as UIElement;
                if (element != null)
                    button.Click += delegate
                    {
                        element.Visibility = element.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    };
            }
        }

        /// <summary>
        /// Set <see cref="ElementProperty"/>
        /// </summary>
        public static void SetElement(DependencyObject element, UIElement value)
        {
            element.SetValue(ElementProperty, value);
        }

        /// <summary>
        /// Get <see cref="ElementProperty"/>
        /// </summary>
        public static UIElement GetElement(DependencyObject element)
        {
            return (UIElement)element.GetValue(ElementProperty);
        }
    }
}
