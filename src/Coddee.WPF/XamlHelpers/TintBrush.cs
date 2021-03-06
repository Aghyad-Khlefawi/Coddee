﻿using System;
using System.Windows.Markup;
using System.Windows.Media;
namespace Coddee.WPF.XamlHelpers
{
    /// <summary>
    /// The return type available for the MarkupExtension
    /// </summary>
    public enum TintBrushReturn
    {
        /// <summary>
        /// Returns a <see cref="SolidColorBrush"/> object
        /// </summary>
        Brush,

        /// <summary>
        /// Returns a <see cref="Color"/> object
        /// </summary>
        Color
    }

    /// <summary>
    /// Returns a tinted shade of a brush.
    /// </summary>
    public class TintBrush : MarkupExtension
    {

        /// <summary>
        /// Specifies the return type of the extension
        /// </summary>
        public TintBrushReturn Return { get; set; }

        /// <summary>
        /// The target bush
        /// </summary>
        public SolidColorBrush Brush { get; set; }

        /// <summary>
        /// Amount of added tint
        /// </summary>
        public float Amount { get; set; }

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var color = DarkenColor();
            if (Return == TintBrushReturn.Brush)
                return new SolidColorBrush(color);
            return color;
        }

        Color DarkenColor()
        {
            var Color = Brush.Color;
            float red = (float)Color.R;
            float green = (float)Color.G;
            float blue = (float)Color.B;

            red = (255 - red) * Amount + red;
            green = (255 - green) * Amount + green;
            blue = (255 - blue) * Amount + blue;

            return Color.FromArgb(Color.A, (byte)red, (byte)green, (byte)blue);
        }
    }
}
