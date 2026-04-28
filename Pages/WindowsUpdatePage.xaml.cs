using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TrickLor.Services;

namespace TrickLor.Pages
{
    public partial class WindowsUpdatePage : Page
    {
        public WindowsUpdatePage() => InitializeComponent();

        private void Page_Loaded(object sender, RoutedEventArgs e) => RefreshStatus();

        private void RefreshStatus()
        {
            bool paused = WindowsUpdateService.IsPaused();
            if (paused)
            {
                TxtStatusIcon.Text   = "⏸";
                TxtUpdateStatus.Text = "Cập nhật đang bị tạm dừng";
                TxtUpdateDesc.Text   = "Windows sẽ không tự động tải và cài bản cập nhật";
                StatusCard.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0xF7, 0xED));
            }
            else
            {
                TxtStatusIcon.Text   = "✅";
                TxtUpdateStatus.Text = "Cập nhật đang hoạt động bình thường";
                TxtUpdateDesc.Text   = "Windows có thể tự động tải và cài bản cập nhật";
                StatusCard.Background = new SolidColorBrush(Color.FromRgb(0xF0, 0xFD, 0xF4));
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            WindowsUpdateService.Pause();
            LogService.Add("Windows Update: Đã tạm dừng cập nhật tự động");
            TxtStatus.Text = "✅ Đã tạm dừng Windows Update";
            RefreshStatus();
        }

        private void Resume_Click(object sender, RoutedEventArgs e)
        {
            WindowsUpdateService.Resume();
            LogService.Add("Windows Update: Đã bật lại cập nhật tự động");
            TxtStatus.Text = "✅ Đã bật lại Windows Update";
            RefreshStatus();
        }

        private async void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            await WindowsUpdateService.OpenSettingsAsync();
        }
    }
}
