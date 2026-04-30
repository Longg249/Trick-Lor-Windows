using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using TrickLor.Services;
using Windows.UI;

namespace TrickLor.Pages
{
    public sealed partial class WindowsUpdatePage : Page
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
                StatusCard.Background = new SolidColorBrush(Color.FromArgb(255, 0xFF, 0xF7, 0xED));
            }
            else
            {
                TxtStatusIcon.Text   = "✅";
                TxtUpdateStatus.Text = "Cập nhật đang hoạt động bình thường";
                TxtUpdateDesc.Text   = "Windows có thể tự động tải và cài bản cập nhật";
                StatusCard.Background = new SolidColorBrush(Color.FromArgb(255, 0xF0, 0xFD, 0xF4));
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
            => await WindowsUpdateService.OpenSettingsAsync();
    }
}
