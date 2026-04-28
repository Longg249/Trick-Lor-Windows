using System.Windows;
using System.Windows.Controls;
using TrickLor.Services;

namespace TrickLor.Pages
{
    public partial class EventLogPage : Page
    {
        public EventLogPage() => InitializeComponent();

        private async void Page_Loaded(object sender, RoutedEventArgs e) => await LoadAsync();

        private async void Load_Click(object sender, RoutedEventArgs e) => await LoadAsync();

        private async System.Threading.Tasks.Task LoadAsync()
        {
            TxtLoading.Visibility = Visibility.Visible;
            DgEvents.Visibility   = Visibility.Collapsed;

            var logName    = (CmbLog.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "System";
            bool errOnly   = ChkErrorsOnly.IsChecked == true;
            TxtStatus.Text = $"⏳ Đang đọc {logName} log...";

            var list = await EventLogViewerService.GetEntriesAsync(logName, errOnly);
            DgEvents.ItemsSource  = list;

            TxtLoading.Visibility = Visibility.Collapsed;
            DgEvents.Visibility   = Visibility.Visible;
            TxtStatus.Text = $"{logName}: {list.Count} mục{(errOnly ? " (chỉ lỗi & cảnh báo)" : "")}";
        }
    }
}
