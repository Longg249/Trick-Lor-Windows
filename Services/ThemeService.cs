using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace TrickLor.Services
{
    public static class ThemeService
    {
        // ── State ────────────────────────────────────────────────────────────
        public static bool IsDark { get; private set; } = false;
        public static string AccentHex { get; private set; } = "#3B82F6";

        // ── Event: Action<bool> — nhận isDark ────────────────────────────────
        // MainWindow đăng ký: ThemeService.ThemeChanged += OnThemeChanged(bool isDark)
        public static event Action<bool>? ThemeChanged;

        // ── Apply dark/light mode ─────────────────────────────────────────────
        // Gọi từ: ThemeService.Apply(bool)  — App.xaml.cs, MainWindow, SettingsPage
        public static void Apply(bool dark)
        {
            IsDark = dark;

            // Áp dụng theme cho MainWindow nếu đã khởi tạo
            if (App.MainWindow?.Content is FrameworkElement root)
                root.RequestedTheme = dark ? ElementTheme.Dark : ElementTheme.Light;

            ThemeChanged?.Invoke(dark);
        }

        // ── Apply accent color ────────────────────────────────────────────────
        // Gọi từ: ThemeService.ApplyAccent(string hex)
        public static void ApplyAccent(string hex)
        {
            AccentHex = hex;

            try
            {
                var color = ParseColor(hex);
                var res = Application.Current.Resources;
                res["BrushAccent"]     = new SolidColorBrush(color);
                res["BrushAccentBlue"] = new SolidColorBrush(color);
                res["AccentColor"]     = color;
            }
            catch { }
        }

        // ── Parse hex → Color (không nullable) ────────────────────────────────
        // Gọi từ: new SolidColorBrush(ThemeService.ParseColor(hex)) — SettingsPage
        // Ném exception nếu không hợp lệ để caller có thể try/catch
        public static Color ParseColor(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                throw new ArgumentException("Hex color rỗng");

            hex = hex.TrimStart('#');

            if (hex.Length == 6)
            {
                byte r = Convert.ToByte(hex[0..2], 16);
                byte g = Convert.ToByte(hex[2..4], 16);
                byte b = Convert.ToByte(hex[4..6], 16);
                return Color.FromArgb(255, r, g, b);
            }
            else if (hex.Length == 8)
            {
                byte a = Convert.ToByte(hex[0..2], 16);
                byte r = Convert.ToByte(hex[2..4], 16);
                byte g = Convert.ToByte(hex[4..6], 16);
                byte b = Convert.ToByte(hex[6..8], 16);
                return Color.FromArgb(a, r, g, b);
            }

            throw new FormatException($"Mã màu không hợp lệ: #{hex}");
        }
    }
}
