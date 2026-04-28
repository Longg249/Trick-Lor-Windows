using System.Windows;
using System.Windows.Controls;
using TrickLor.Services;

namespace TrickLor.Pages
{
    public partial class OptimizePage : Page
    {
        public OptimizePage()
        {
            InitializeComponent();
        }

        private async void ApplyPerf_Click(object sender, RoutedEventArgs e)
        {
            BtnApplyPerf.IsEnabled = false;
            TxtOptStatus.Text = "⏳ Đang áp dụng cài đặt Hiệu suất...";

            var opts = new OptimizeOptions
            {
                VisualFX      = TglVisualFX.IsChecked == true,
                PowerPlan     = TglPowerPlan.IsChecked == true,
                SysMain       = TglSysMain.IsChecked == true,
                SearchIndex   = TglSearchIndex.IsChecked == true,
            };

            var progress = new System.Progress<string>(msg => TxtOptStatus.Text = msg);
            await OptimizeService.ApplyPerformanceAsync(opts, progress);

            TxtOptStatus.Text = "✅ Cài đặt Hiệu suất đã áp dụng thành công!";
            BtnApplyPerf.IsEnabled = true;
            LogService.Add("Optimize: Hiệu suất áp dụng");
        }

        private async void ApplyPrivacy_Click(object sender, RoutedEventArgs e)
        {
            BtnApplyPrivacy.IsEnabled = false;
            TxtOptStatus.Text = "⏳ Đang áp dụng cài đặt Quyền riêng tư...";

            var opts = new PrivacyOptions
            {
                Telemetry   = TglTelemetry.IsChecked == true,
                Cortana     = TglCortana.IsChecked == true,
                AdvertisingId = TglAdID.IsChecked == true,
                ActivityHistory = TglActivity.IsChecked == true,
            };

            var progress = new System.Progress<string>(msg => TxtOptStatus.Text = msg);
            await OptimizeService.ApplyPrivacyAsync(opts, progress);

            TxtOptStatus.Text = "✅ Cài đặt Quyền riêng tư đã áp dụng!";
            BtnApplyPrivacy.IsEnabled = true;
            LogService.Add("Optimize: Privacy áp dụng");
        }

        private async void ApplyStorage_Click(object sender, RoutedEventArgs e)
        {
            BtnApplyStorage.IsEnabled = false;
            TxtOptStatus.Text = "⏳ Đang dọn dẹp bộ nhớ...";

            var opts = new StorageOptions
            {
                TempFiles   = TglTemp.IsChecked == true,
                RecycleBin  = TglRecycle.IsChecked == true,
                Hibernation = TglHibernate.IsChecked == true,
            };

            var progress = new System.Progress<string>(msg => TxtOptStatus.Text = msg);
            await OptimizeService.ApplyStorageAsync(opts, progress);

            TxtOptStatus.Text = "✅ Dọn dẹp hoàn tất!";
            BtnApplyStorage.IsEnabled = true;
            LogService.Add("Optimize: Storage dọn dẹp xong");
        }

        private async void ApplyStartup_Click(object sender, RoutedEventArgs e)
        {
            BtnApplyStartup.IsEnabled = false;
            TxtOptStatus.Text = "⏳ Đang cấu hình Startup...";

            var opts = new StartupOptions
            {
                FastBoot  = TglFastBoot.IsChecked == true,
                GameBar   = TglGameBar.IsChecked == true,
                FlushDns  = TglFlushDns.IsChecked == true,
            };

            var progress = new System.Progress<string>(msg => TxtOptStatus.Text = msg);
            await OptimizeService.ApplyStartupAsync(opts, progress);

            TxtOptStatus.Text = "✅ Startup đã cấu hình xong!";
            BtnApplyStartup.IsEnabled = true;
            LogService.Add("Optimize: Startup cấu hình xong");
        }
    }
}
