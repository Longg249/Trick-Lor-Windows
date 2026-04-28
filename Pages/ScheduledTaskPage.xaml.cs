using System.Windows;
using System.Windows.Controls;
using TrickLor.Services;

namespace TrickLor.Pages
{
    public partial class ScheduledTaskPage : Page
    {
        public ScheduledTaskPage() => InitializeComponent();

        private async void Page_Loaded(object sender, RoutedEventArgs e) => await LoadAsync();

        private async void Refresh_Click(object sender, RoutedEventArgs e) => await LoadAsync();

        private async System.Threading.Tasks.Task LoadAsync()
        {
            TxtLoading.Visibility = Visibility.Visible;
            DgTasks.Visibility    = Visibility.Collapsed;
            TxtStatus.Text        = "⏳ Đang truy vấn schtasks...";

            var list = await ScheduledTaskService.GetTasksAsync();
            DgTasks.ItemsSource   = list;

            TxtLoading.Visibility = Visibility.Collapsed;
            DgTasks.Visibility    = Visibility.Visible;
            TxtStatus.Text        = $"Tổng: {list.Count} tác vụ";
        }

        private ScheduledTaskEntry? Selected => DgTasks.SelectedItem as ScheduledTaskEntry;

        private async void Enable_Click(object sender, RoutedEventArgs e)
        {
            if (Selected is not { } t) return;
            await ScheduledTaskService.EnableAsync(t.TaskName);
            LogService.Add($"Scheduled Tasks: Đã bật '{t.TaskName}'");
            await LoadAsync();
        }

        private async void Disable_Click(object sender, RoutedEventArgs e)
        {
            if (Selected is not { } t) return;
            await ScheduledTaskService.DisableAsync(t.TaskName);
            LogService.Add($"Scheduled Tasks: Đã tắt '{t.TaskName}'");
            await LoadAsync();
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Selected is not { } t) return;
            var res = MessageBox.Show($"Xoá tác vụ '{t.TaskName}'?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res != MessageBoxResult.Yes) return;
            await ScheduledTaskService.DeleteAsync(t.TaskName);
            LogService.Add($"Scheduled Tasks: Đã xoá '{t.TaskName}'");
            await LoadAsync();
        }
    }
}
