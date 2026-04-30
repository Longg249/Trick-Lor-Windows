using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using TrickLor.Helpers;
using TrickLor.Services;
using Windows.UI;

namespace TrickLor.Pages
{
    public sealed partial class BackupRestorePage : Page
    {
        private string _importPath = "";

        public BackupRestorePage() => InitializeComponent();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TxtSettingsPath.Text = BackupRestoreService.SettingsPath;
            bool exists = BackupRestoreService.SettingsExist();
            TxtSettingsExist.Text       = exists ? "✅ File cài đặt tồn tại" : "⚠️ Chưa có file cài đặt";
            TxtSettingsExist.Foreground = exists
                ? new SolidColorBrush(Color.FromArgb(255, 0x22, 0xC5, 0x5E))
                : new SolidColorBrush(Color.FromArgb(255, 0xF5, 0x9E, 0x0B));
        }

        private async void Export_Click(object sender, RoutedEventArgs e)
        {
            if (!BackupRestoreService.SettingsExist()) { TxtStatus.Text = "⚠️ Chưa có file cài đặt để xuất"; return; }
            var path = await DialogHelper.SaveJsonAsync("TrickLor_settings.json");
            if (path == null) return;
            bool ok = BackupRestoreService.Export(path);
            TxtStatus.Text = ok ? $"✅ Đã xuất cài đặt ra: {path}" : "❌ Xuất thất bại";
            if (ok) LogService.Add($"Backup: Đã xuất cài đặt ra {path}");
        }

        private async void BrowseImport_Click(object sender, RoutedEventArgs e)
        {
            var path = await DialogHelper.OpenJsonAsync();
            if (path == null) return;
            _importPath         = path;
            TxtImportPath.Text  = _importPath;
            BtnImport.IsEnabled = true;
        }

        private async void Import_Click(object sender, RoutedEventArgs e)
        {
            bool confirmed = await DialogHelper.ConfirmAsync(
                "Xác nhận",
                "Nhập cài đặt sẽ ghi đè cài đặt hiện tại. Tiếp tục?",
                XamlRoot);
            if (!confirmed) return;
            bool ok = BackupRestoreService.Import(_importPath);
            TxtStatus.Text = ok ? "✅ Nhập thành công! Khởi động lại để áp dụng." : "❌ Nhập thất bại — kiểm tra file JSON";
            if (ok) LogService.Add($"Restore: Đã nhập cài đặt từ {_importPath}");
        }
    }
}
