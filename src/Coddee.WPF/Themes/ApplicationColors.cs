using System.Windows.Media;

namespace Coddee.WPF
{
    /// <summary>
    /// Contains the color scheme used in the application
    /// </summary>
    public static class ApplicationColors
    {

        /// <summary>
        /// The accent color used by the application controls
        /// </summary>
        public static SolidColorBrush ApplicationAccentColor { get; set; } = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#065FCF"));
        
        /// <summary>
        /// A darker shade of the <see cref="ApplicationAccentColor"/>
        /// </summary>
        public static SolidColorBrush ApplicationAccentColorDarker { get; set; } = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#06438F"));

        /// <summary>
        /// The color of the navigation bar border
        /// </summary>
        public static SolidColorBrush NavigationBarBorderBrush { get; set; } = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#065FCF"));

        /// <summary>
        /// The background color of the navigation bar.
        /// </summary>
        public static SolidColorBrush NavbarBackground { get; set; } = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));
    }
}
