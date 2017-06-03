// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Concord.HIS1.UI.Converters;

namespace Coddee.WPF
{
    public class Validate
    {
        public static readonly DependencyProperty FieldNameProperty = DependencyProperty.RegisterAttached(
                                                                "FieldName",
                                                                typeof(string),
                                                                typeof(Validate),
                                                                new PropertyMetadata(default(string),ValueSet));

        private static void ValueSet(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Control control)
            {
                var value = e.NewValue as string;
                var binding = new MultiBinding
                {
                    Converter = new RequiredBrushConverter(),
                    ConverterParameter = value
                };
                binding.Bindings.Add(new Binding(nameof(ViewModelBase.RequiredFields)));
                binding.Bindings.Add(new Binding(value));
                control.SetBinding(Control.BorderBrushProperty, binding);
            }
        }

        public static void SetFieldName(DependencyObject element, string value)
        {
            element.SetValue(FieldNameProperty, value);
            
        }

        public static string GetFieldName(DependencyObject element)
        {
            return (string) element.GetValue(FieldNameProperty);
        }
    }
}
