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
    /// <summary>
    /// Uses the ViewModel validation rules and returns a red brush if the value is invalid.
    /// </summary>
    public class RequiredBrushConverter : IMultiValueConverter
    {
        /// <summary>
        /// Color used if the validation returned error result.
        /// </summary>
        public static SolidColorBrush ErrorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D91D1D"));

        /// <summary>
        /// Color used if the validation returned warning result.
        /// </summary>
        public static SolidColorBrush WarningBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF39524"));

        /// <summary>
        /// Color used if the validation succeeded.
        /// </summary>
        public static SolidColorBrush GrayBrush = System.Windows.SystemColors.ActiveBorderBrush;

        /// <inheritdoc />
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

        /// <inheritdoc />
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}