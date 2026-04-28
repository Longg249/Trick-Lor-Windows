using System.Windows;
using System.Windows.Controls;
using TrickLor.Services;

namespace TrickLor.Pages
{
    public partial class DiskCleanerPage : Page
    {
        public DiskCleanerPage() => InitializeComponent();

        private DiskCleanOptions BuildOpts() => new()
        {
            WinUpdateCache = TglWinUpdate.IsChecked == true,
            Prefetch       = TglPrefetch.IsChecked  == true,
            WinErrorReport = TglWER.IsChecked        == true,
            ThumbnailCache = TglThumb.IsChecked      == true,
            TempFiles      = TglTemp.IsChecked       == true,
            RecycleBin     = TglRecycle.IsChecked    == true,
        };

        private async void Estimate_Click(object sender, RoutedEventArgs e)
        {
            TxtStatus.Text   = "⏳ Đang tính toán dung lượng...";
            TxtEstimate.Text = "...";
            long bytes = await DiskCleanerService.EstimateAsync(BuildOpts());
            TxtEstimate.Text = DiskCleanerService.FormatSize(bytes);
            TxtStatus.Text   = $"Ước tính: {DiskCleanerService.FormatSize(bytes)} có thể giải phóng";
        }

        private async void Clean_Click(object sender, RoutedEventArgs e)
        {
            BtnClean.IsEnabled = false;
            var progress = new System.Progress<string>(msg => TxtStatus.Text = msg);
            await DiskCleanerService.CleanAsync(BuildOpts(), progress);
            TxtStatus.Text     = "✅ Dọn dẹp hoàn tất!";
            BtnClean.IsEnabled = true;
            LogService.Add("Disk Cleaner: Dọn dẹp file rác hoàn tất");
        }
    }
}
