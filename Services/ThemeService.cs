using System;
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
            ("BrushBg",          "#0F172A", "#F8FAFC"),
            ("BrushSidebar",     "#1E293B", "#FFFFFF"),
            ("BrushCard",        "#1E2D3D", "#FFFFFF"),
            ("BrushCardHover",   "#253649", "#F1F5F9"),
            ("BrushBorder",      "#334155", "#E2E8F0"),
            ("BrushBorderLight", "#475569", "#CBD5E1"),
            ("BrushText",        "#F1F5F9", "#0F172A"),
            ("BrushMuted",       "#94A3B8", "#64748B"),
            ("BrushSubtle",      "#334155", "#E2E8F0"),
            ("BrushNavHover",    "#253649", "#EFF6FF"),
        };

        public static event Action<bool>? ThemeChanged;

        public static void Apply(bool dark)
        {
            IsDark = dark;
            SettingsService.IsDarkMode = dark;
            var res = Application.Current.Resources;

            foreach (var (key, darkHex, lightHex) in Palette)
            {
                var hex = dark ? darkHex : lightHex;
                res[key] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
            }

            // Re-apply accent so it isn't overwritten by palette reset
            var accent = SettingsService.AccentColor;
            if (!string.IsNullOrEmpty(accent))
                res["BrushAccentBlue"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(accent));

            ThemeChanged?.Invoke(dark);
        }

        public static void Toggle() => Apply(!IsDark);

        public static void ApplyAccent(string hex)
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(hex);
                Application.Current.Resources["BrushAccentBlue"] = new SolidColorBrush(color);
                SettingsService.AccentColor = hex;
            }
            catch { }
        }
    }
}
