using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using TrickLor.Services;

namespace TrickLor.Pages
{
    public sealed partial class SettingsPage : Page
    {
        private bool _loading = true;

        public SettingsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _loading = true;
            ChkDarkMode.IsOn = ThemeService.IsDark;
            var accent = SettingsService.AccentColor;
            TxtColorInput.Text = accent;
            RefreshCurrentSwatch(accent);
            _loading = false;
        }

        private void RefreshCurrentSwatch(string hex)
        {
            try
            {
                var brush = new SolidColorBrush(ThemeService.ParseColor(hex));
                CurrentSwatch.Background = brush;
                InputSwatch.Background   = brush;
                TxtCurrentColor.Text     = hex.ToUpperInvariant();
            }
            catch { }
        }

        private void ColorInput_Changed(object sender, TextChangedEventArgs e)
        {
            if (_loading) return;
            var text = TxtColorInput.Text.Trim();
            if (!text.StartsWith('#')) text = "#" + text;
            if (text.Length == 7)
            {
                try { InputSwatch.Background = new SolidColorBrush(ThemeService.ParseColor(text)); }
                catch { }
            }
        }

        private void ApplyColor_Click(object sender, RoutedEventArgs e)
        {
            var hex = TxtColorInput.Text.Trim().ToUpperInvariant();
            if (!hex.StartsWith('#')) hex = "#" + hex;
            try
            {
                ThemeService.ParseColor(hex);
                ThemeService.ApplyAccent(hex);
                RefreshCurrentSwatch(hex);
                TxtStatus.Text = $"✅ Đã áp dụng màu {hex}";
                LogService.Add($"Settings: Đổi màu nhấn → {hex}");
            }
            catch
            {
                TxtStatus.Text = "❌ Mã màu không hợp lệ — ví dụ: #3B82F6";
            }
        }

        private void ResetColor_Click(object sender, RoutedEventArgs e)
        {
            const string def = "#3B82F6";
            TxtColorInput.Text = def;
            ThemeService.ApplyAccent(def);
            RefreshCurrentSwatch(def);
            TxtStatus.Text = "✅ Đã đặt lại về màu mặc định";
            LogService.Add("Settings: Đặt lại màu nhấn về mặc định");
        }

        private void Preset_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: string hex })
            {
                TxtColorInput.Text = hex;
                ThemeService.ApplyAccent(hex);
                RefreshCurrentSwatch(hex);
                TxtStatus.Text = $"✅ Đã áp dụng màu {hex}";
                LogService.Add($"Settings: Đổi màu nhấn → {hex}");
            }
        }

        private void DarkMode_Toggled(object sender, RoutedEventArgs e)
        {
            if (_loading) return;
            bool isDark = ChkDarkMode.IsOn;
            ThemeService.Apply(isDark);
            TxtStatus.Text = isDark ? "✅ Đã bật giao diện tối" : "✅ Đã bật giao diện sáng";
        }
    }
}
