using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using TrickLor.Services;

namespace TrickLor.Pages
{
    public partial class BackupRestorePage : Page
    {
        private string _importPath = "";

        public BackupRestorePage() => InitializeComponent();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TxtSettingsPath.Text = BackupRestoreService.SettingsPath;
            bool exists = BackupRestoreService.SettingsExist();
            TxtSettingsExist.Text       = exists ? "✅ File cài đặt tồn tại" : "⚠️ Chưa có file cài đặt (chưa lưu lần nào)";
            TxtSettingsExist.Foreground = exists
                ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x22, 0xC5, 0x5E))
                : new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xF5, 0x9E, 0x0B));
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            if (!BackupRestoreService.SettingsExist())
            {
                TxtStatus.Text = "⚠️ Chưa có file cài đặt để xuất";
                return;
            }
            var dlg = new SaveFileDialog { Filter = "JSON Files|*.json", FileName = "TrickLor_settings.json" };
            if (dlg.ShowDialog() != true) return;
            bool ok = BackupRestoreService.Export(dlg.FileName);
            TxtStatus.Text = ok ? $"✅ Đã xuất cài đặt ra: {dlg.FileName}" : "❌ Xuất thất bại";
            if (ok) LogService.Add($"Backup: Đã xuất cài đặt ra {dlg.FileName}");
        }

        private void BrowseImport_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "JSON Files|*.json" };
            if (dlg.ShowDialog() != true) return;
            _importPath         = dlg.FileName;
            TxtImportPath.Text  = _importPath;
            BtnImport.IsEnabled = true;
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Nhập cài đặt sẽ ghi đè cài đặt hiện tại. Tiếp tục?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;
            bool ok = BackupRestoreService.Import(_importPath);
            TxtStatus.Text = ok ? "✅ Nhập thành công! Khởi động lại để áp dụng." : "❌ Nhập thất bại — kiểm tra file JSON";
            if (ok) LogService.Add($"Restore: Đã nhập cài đặt từ {_importPath}");
        }
    }
}
