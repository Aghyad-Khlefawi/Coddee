using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Coddee.WPF
{
    public enum InputType
    {
        Alphabetic,
        Numeric
    }
    public class InputMask
    {
        public static readonly DependencyProperty InputTypeProperty = DependencyProperty.RegisterAttached(
                                                                "InputType",
                                                                typeof(InputType?),
                                                                typeof(InputMask),
                                                                new PropertyMetadata(default(InputType?),MaskTypeSet));

        private static void MaskTypeSet(DependencyObject d, DependencyPropertyChangedEventArgs arg)
        {
            if (d is TextBoxBase textBox && arg.NewValue!=null)
            {
                Func<string, bool> func = (InputType) arg.NewValue == InputType.Alphabetic
                    ? (Func<string, bool>)(e=>!Regex.IsMatch(e, @"^[A-Z a-z ء-ي]+$"))
                    : e=>!Regex.IsMatch(e, @"^[\d]+$");

                textBox.PreviewTextInput+=(sender,e)=>e.Handled = func(e.Text);
            }
        }

        public static void SetInputType(DependencyObject element, InputType? value)
        {
            element.SetValue(InputTypeProperty, value);
        }

        public static InputType? GetInputType(DependencyObject element)
        {
            return (InputType?) element.GetValue(InputTypeProperty);
        }
    }
}
