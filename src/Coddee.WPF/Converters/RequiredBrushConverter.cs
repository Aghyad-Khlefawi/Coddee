// Copyright (c) Aghyad khlefawi. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using Coddee.Validation;

namespace Coddee.WPF.Converters
{
    public class RequiredBrushConverter : IMultiValueConverter
    {
        public static SolidColorBrush ErrorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D91D1D"));
        public static SolidColorBrush WarningBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF39524"));
        public static SolidColorBrush GrayBrush = System.Windows.SystemColors.ActiveBorderBrush;
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is List<IValidationRule> required && parameter is string fieldName)
            {
                var fields = required.Where(e => e.FieldName == fieldName);
                foreach (var field in fields)
                    if (!field.Validate())
                    {
                        if (field.ValidationType == ValidationType.Error)
                            return ErrorBrush;
                        return WarningBrush;
                    }
            }
            return GrayBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}