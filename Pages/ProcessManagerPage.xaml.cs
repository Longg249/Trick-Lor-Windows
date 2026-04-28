using System.Windows;
using System.Windows.Controls;
using TrickLor.Services;

namespace TrickLor.Pages
{
    public partial class ProcessManagerPage : Page
    {
        public ProcessManagerPage() => InitializeComponent();

        private async void Page_Loaded(object sender, RoutedEventArgs e) => await LoadAsync();

        private async void Refresh_Click(object sender, RoutedEventArgs e) => await LoadAsync();

        private async System.Threading.Tasks.Task LoadAsync()
        {
            TxtStatus.Text = "⏳ Đang tải...";
            var list = await ProcessManagerService.GetProcessesAsync();
            DgProcs.ItemsSource = list;
            TxtStatus.Text = $"{list.Count} tiến trình đang chạy — sắp xếp theo RAM (cao → thấp)";
        }

        private async void Kill_Click(object sender, RoutedEventArgs e)
        {
            if (DgProcs.SelectedItem is not ProcessEntry entry) return;
            var result = MessageBox.Show(
                $"Kết thúc tiến trình '{entry.Name}' (PID {entry.PID})?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
            ProcessManagerService.Kill(entry.PID);
            LogService.Add($"Process Manager: Đã kết thúc '{entry.Name}' (PID {entry.PID})");
            await LoadAsync();
        }
    }
}
