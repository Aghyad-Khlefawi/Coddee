using System;
using System.Windows.Markup;
using System.Windows.Media;
namespace Coddee.WPF.XamlHelpers
{
    /// <summary>
    /// Returns a tinted shade of a brush.
    /// </summary>
    public class TintBrush : MarkupExtension
    {
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
            return new SolidColorBrush(DarkenColor());
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
