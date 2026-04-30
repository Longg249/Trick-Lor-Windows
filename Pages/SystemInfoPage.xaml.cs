using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using TrickLor.Services;
using Windows.UI;

namespace TrickLor.Pages
{
    public partial class SystemInfoPage : Page
    {
        private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(2) };

        public SystemInfoPage()
        {
            InitializeComponent();
            _timer.Tick += async (_, _) => await RefreshCpuAsync();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStaticInfo();
            LoadDiskAndRam();
            LoadBattery();
            await RefreshCpuAsync();
            _timer.Start();
            TxtSysStatus.Text = $"Cập nhật lần cuối: {DateTime.Now:HH:mm:ss}";
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e) => _timer.Stop();

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadDiskAndRam();
            LoadBattery();
            await RefreshCpuAsync();
            TxtSysStatus.Text = $"Cập nhật lần cuối: {DateTime.Now:HH:mm:ss}";
        }

        private void LoadStaticInfo()
        {
            TxtMachineName.Text = SystemInfoService.GetMachineName();
            TxtMachineId.Text   = SystemInfoService.GetMachineId();
            TxtTimezone.Text    = SystemInfoService.GetTimeZone();
            TxtOS.Text          = SystemInfoService.GetOSVersion();
            TxtCpuName.Text     = SystemInfoService.GetCpuName();
            TxtGpuName.Text     = SystemInfoService.GetGpuName();
        }

        private void LoadDiskAndRam()
        {
            var ram  = SystemInfoService.GetRam();
            double pct = ram.UsedPercent;
            BarRam.Value         = pct;
            BarRam.Foreground    = PickColor(pct);
            TxtRamPct.Text       = $"{pct:F0}%";
            TxtRamPct.Foreground = PickColor(pct);
            TxtRamDetail.Text    = $"{ToGB(ram.UsedBytes):F1} GB / {ToGB(ram.TotalBytes):F1} GB";
            TxtRamUsed.Text      = $"Đã dùng: {ToGB(ram.UsedBytes):F1} GB";
            TxtRamFree.Text      = $"Còn trống: {ToGB(ram.TotalBytes - ram.UsedBytes):F1} GB";

            var disks = SystemInfoService.GetDisks();
            foreach (var d in disks)
            {
                if (!d.Drive.StartsWith("C", StringComparison.OrdinalIgnoreCase)) continue;
                double dp = d.UsedPercent;
                BarDisk.Value         = dp;
                BarDisk.Foreground    = PickColor(dp);
                TxtDiskPct.Text       = $"{dp:F0}%";
                TxtDiskPct.Foreground = PickColor(dp);
                TxtDiskDetail.Text    = $"{ToGB(d.TotalBytes - d.FreeBytes):F0} GB / {ToGB(d.TotalBytes):F0} GB";
                TxtDiskUsed.Text      = $"Đã dùng: {ToGB(d.TotalBytes - d.FreeBytes):F0} GB";
                TxtDiskFree.Text      = $"Còn trống: {ToGB(d.FreeBytes):F0} GB";
                break;
            }
        }

        private void LoadBattery()
        {
            var bat = SystemInfoService.GetBattery();
            if (!bat.Present)
            {
                TxtBatPct.Text    = "—";
                TxtBatDetail.Text = "Không có pin (PC để bàn)";
                TxtBatStatus.Text = "Nguồn AC";
                BarBat.Value      = 0;
                return;
            }

            BarBat.Value         = bat.Percent;
            BarBat.Foreground    = PickColor(100 - bat.Percent);
            TxtBatPct.Text       = $"{bat.Percent}%";
            TxtBatPct.Foreground = PickColor(100 - bat.Percent);
            TxtBatDetail.Text    = bat.IsCharging ? "Đang sạc" : "Dùng pin";
            TxtBatStatus.Text    = bat.IsCharging ? "⚡ Đang kết nối nguồn điện" : "🔋 Đang chạy bằng pin";
        }

        private async System.Threading.Tasks.Task RefreshCpuAsync()
        {
            float cpu = await SystemInfoService.GetCpuAsync();
            BarCpu.Value         = cpu;
            BarCpu.Foreground    = PickColor(cpu);
            TxtCpuPct.Text       = $"{cpu:F0}%";
            TxtCpuPct.Foreground = PickColor(cpu);
            TxtCpuDetail.Text    = cpu switch
            {
                < 40 => "Nhàn rỗi",
                < 70 => "Hoạt động bình thường",
                < 90 => "Tải cao",
                _    => "Quá tải!"
            };
        }

        private static double ToGB(long bytes) => bytes / 1_073_741_824.0;

        private static SolidColorBrush PickColor(double pct) => pct switch
        {
            < 60 => new SolidColorBrush(Color.FromArgb(255, 0x3B, 0x82, 0xF6)),
            < 85 => new SolidColorBrush(Color.FromArgb(255, 0xF5, 0x9E, 0x0B)),
            _    => new SolidColorBrush(Color.FromArgb(255, 0xEF, 0x44, 0x44))
        };
    }
}
