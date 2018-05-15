using System.Windows.Media;

namespace Coddee.WPF
{
    /// <summary>
    /// Defines the colors used in a WPF application
    /// </summary>
    public class ApplicationColors
    {
        /// <summary>
        /// The accent color used by the application controls
        /// </summary>
        public SolidColorBrush ApplicationAccentColor { get; set; }

        /// <summary>
        /// A darker shade of the <see cref="ApplicationAccentColor"/>
        /// </summary>
        public SolidColorBrush ApplicationAccentColorDarker { get; set; }

        /// <summary>
        /// The color of the navigation bar border
        /// </summary>
        public SolidColorBrush NavigationBarBorderBrush { get; set; }

        /// <summary>
        /// The background color of the navigation bar.
        /// </summary>
        public SolidColorBrush NavbarBackground { get; set; }

        /// <summary>
        /// The default Coddee application theme
        /// </summary>
        public static ApplicationColors Default { get; } = new ApplicationColors
        {
            ApplicationAccentColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1987c4")),
            ApplicationAccentColorDarker = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10729d")),
            NavigationBarBorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10729d")),
            NavbarBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"))
        };

    }

    /// <summary>
    /// Manages the application theme
    /// </summary>
    public static class ApplicationTheme
    {
        /// <summary>
        /// The accent color used by the application controls
        /// </summary>
        public static SolidColorBrush ApplicationAccentColor { get; private set; }

        /// <summary>
        /// A darker shade of the <see cref="ApplicationAccentColor"/>
        /// </summary>
        public static SolidColorBrush ApplicationAccentColorDarker { get; private set; } 

        /// <summary>
        /// The color of the navigation bar border
        /// </summary>
        public static SolidColorBrush NavigationBarBorderBrush { get; private set; }

        /// <summary>
        /// The background color of the navigation bar.
        /// </summary>
        public static SolidColorBrush NavbarBackground { get; private set; }


        static ApplicationTheme()
        {
            SetTheme(ApplicationColors.Default);
        }

        /// <summary>
        /// Set a new Application theme
        /// </summary>
        public static void SetTheme(ApplicationColors colors)
        {
            ApplicationAccentColor = colors.ApplicationAccentColor;
            ApplicationAccentColorDarker = colors.ApplicationAccentColorDarker;
            NavbarBackground = colors.NavbarBackground;
            NavigationBarBorderBrush = colors.NavigationBarBorderBrush;

            var app = WPFApplication.Current;
            if (app != null)
            {
                var sysApp = app.GetSystemApplication();
                sysApp.Resources[nameof(ApplicationAccentColor)] = ApplicationAccentColor;
                sysApp.Resources[nameof(ApplicationAccentColorDarker)] = ApplicationAccentColorDarker;
                sysApp.Resources[nameof(NavbarBackground)] = NavbarBackground;
                sysApp.Resources[nameof(NavigationBarBorderBrush)] = NavigationBarBorderBrush;
            }
        }
    }
}
