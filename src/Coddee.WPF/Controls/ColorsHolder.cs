using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Coddee.WPF.Controls
{
    /// <summary>
    /// A helper object for dealing with XAML styles colors changes
    /// </summary>
    public class ColorsHolder : FrameworkElement
    {
        /// <summary>
        /// Color slot.
        /// </summary>
        public static readonly DependencyProperty Color1Property = DependencyProperty.Register(
                                                        "Color1",
                                                        typeof(SolidColorBrush),
                                                        typeof(ColorsHolder),
                                                        new PropertyMetadata(default(SolidColorBrush)));

        /// <summary>
        /// Color slot.
        /// </summary>
        public static readonly DependencyProperty Color2Property = DependencyProperty.Register(
                                                                                               "Color2",
                                                                                               typeof(SolidColorBrush),
                                                                                               typeof(ColorsHolder),
                                                                                               new PropertyMetadata(default(SolidColorBrush)));

        /// <inheritdoc cref="Color1Property"/>
        public SolidColorBrush Color1
        {
            get { return (SolidColorBrush) GetValue(Color1Property); }
            set { SetValue(Color1Property, value); }
        }

         /// <inheritdoc cref="Color2Property"/>
        public SolidColorBrush Color2
        {
            get { return (SolidColorBrush)GetValue(Color2Property); }
            set { SetValue(Color2Property, value); }
        }
    }
}
