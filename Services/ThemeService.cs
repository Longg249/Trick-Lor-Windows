using System.Windows;
using System.Windows.Media;

namespace WinDeployPro.Services
{
    public static class ThemeService
    {
        public static bool IsDark { get; private set; }

            static ThemeService()
            {
                IsDark = SettingsService.IsDarkMode;
            }

            private static readonly (string Key, string Dark, string Light)[] Palette =
        {
            ("BrushBg",          "#000000", "#F1F5F9"),
            ("BrushSidebar",     "#0A0A0A", "#FFFFFF"),
            ("BrushCard",        "#111111", "#FFFFFF"),
            ("BrushCardHover",   "#1A1A1A", "#F8FAFC"),
            ("BrushBorder",      "#222222", "#E2E8F0"),
            ("BrushBorderLight", "#333333", "#CBD5E1"),
            ("BrushText",        "#F1F5F9", "#0F172A"),
            ("BrushMuted",       "#64748B", "#64748B"),
            ("BrushSubtle",      "#2A2A2A", "#CBD5E1"),
        };

        public static void Apply(bool dark)
        {
            IsDark = dark;
            SettingsService.IsDarkMode = dark;
            var res = Application.Current.Resources;

            foreach (var (key, darkHex, lightHex) in Palette)
            {
                var hex = dark ? darkHex : lightHex;
                // Replace with a NEW unfrozen brush — never mutate the existing one
                res[key] = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(hex));
            }

            // Update Frame background
            if (Application.Current.MainWindow is MainWindow mw)
            {
                var hex = dark ? "#000000" : "#F1F5F9";
                mw.MainFrame.Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(hex));
            }
        }

        public static void Toggle() => Apply(!IsDark);
    }
}
