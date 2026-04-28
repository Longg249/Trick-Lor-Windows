using System.Windows;
using System.Windows.Controls;
using TrickLor.Services;

namespace TrickLor.Pages
{
    public partial class StartupManagerPage : Page
    {
        public StartupManagerPage() => InitializeComponent();

        private void Page_Loaded(object sender, RoutedEventArgs e) => Load();

        private void Refresh_Click(object sender, RoutedEventArgs e) => Load();

        private void Load()
        {
            var items = StartupManagerService.GetAll();
            LvStartup.ItemsSource = items;
            TxtStatus.Text = $"Tìm thấy {items.Count} mục startup";
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (LvStartup.SelectedItem is not StartupEntry entry) return;
            var result = MessageBox.Show(
                $"Xoá '{entry.Name}' khỏi danh sách startup?\n\nỨng dụng sẽ không tự khởi động nữa.",
                "Xác nhận xoá", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
            StartupManagerService.Remove(entry);
            LogService.Add($"Startup Manager: Đã xoá '{entry.Name}' khỏi startup");
            Load();
        }
    }
}
