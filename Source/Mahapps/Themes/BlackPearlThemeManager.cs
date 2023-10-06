using System.Windows;
using System.Windows.Media;

using ControlzEx.Theming;

namespace BlackPearl.Mahapps.Themes
{
    public interface IBlackPearlThemeManager
    {
        void DecreaseFontSize();
        Color GetAccentColor();
        void IncreaseFontSize();
        bool IsDarkTheme();
        void SetTheme(Color accentColor, bool isDark);
    }

    public class BlackPearlThemeManager : IBlackPearlThemeManager
    {
        public const string H1FontSize = "H1FontSize";
        public const string H2FontSize = "H2FontSize";
        public const string FontSizeIncrementBy = "FontSizeIncrementBy";
        public const string MaxH1FontSize = "MaxH1FontSize";
        public const string MinH1FontSize = "MinH1FontSize";
        public bool IsDarkTheme()
            => ThemeManager.Current.DetectTheme()?.BaseColorScheme == ThemeManager.BaseColorDarkConst;
        public Color GetAccentColor()
        => (Color)ColorConverter.ConvertFromString(ThemeManager.Current.DetectTheme()?.ColorScheme ?? "Blue");
        public void SetTheme(Color accentColor, bool isDark)
        {
            try
            {
                var baseColor = isDark ? ThemeManager.BaseColorDarkConst : ThemeManager.BaseColorLightConst;
                var theme = RuntimeThemeGenerator.Current.GenerateRuntimeTheme(baseColor, accentColor);
                ThemeManager.Current.ChangeTheme(Application.Current, theme);
            }
            catch { }
        }
        public void IncreaseFontSize()
        {
            var currentValue = (double)Application.Current.Resources[H1FontSize];
            var maxH1FontSize = (double)Application.Current.Resources[MaxH1FontSize];
            var incrementBy = (double)Application.Current.Resources[FontSizeIncrementBy];

            if (currentValue >= maxH1FontSize)
            {
                return;
            }

            var newValue = (currentValue + incrementBy) > maxH1FontSize ? maxH1FontSize : currentValue + incrementBy;

            Application.Current.Resources[H1FontSize] = newValue;
            Application.Current.Resources[H2FontSize] = newValue - incrementBy;
        }
        public void DecreaseFontSize()
        {
            var currentValue = (double)Application.Current.Resources[H1FontSize];
            var minH1FontSize = (double)Application.Current.Resources[MinH1FontSize];
            var incrementBy = (double)Application.Current.Resources[FontSizeIncrementBy];

            if (currentValue <= minH1FontSize)
            {
                return;
            }

            var newValue = (currentValue - incrementBy) < minH1FontSize ? minH1FontSize : currentValue - incrementBy;
            Application.Current.Resources[H1FontSize] = newValue;
            Application.Current.Resources[H2FontSize] = newValue - incrementBy;
        }
    }
}
