using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TrickLor.Helpers;
using TrickLor.Services;

namespace TrickLor.Pages
{
    public sealed partial class StartupManagerPage : Page
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

        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (LvStartup.SelectedItem is not StartupEntry entry) return;
            bool confirmed = await DialogHelper.ConfirmAsync(
                "Xác nhận xoá",
                $"Xoá '{entry.Name}' khỏi danh sách startup?\n\nỨng dụng sẽ không tự khởi động nữa.",
                XamlRoot);
            if (!confirmed) return;
            StartupManagerService.Remove(entry);
            LogService.Add($"Startup Manager: Đã xoá '{entry.Name}' khỏi startup");
            Load();
        }
    }
}
