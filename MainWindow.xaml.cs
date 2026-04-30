using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Windowing;
using TrickLor.Pages;
using TrickLor.Services;
using Windows.Graphics;

namespace TrickLor
{
    public sealed partial class MainWindow : Window
    {
        private bool _updatingTheme;
        private bool _firstNavigationDone = false;

        public MainWindow()
        {
            InitializeComponent();

            // Kích thước và vị trí window
            var appWindow = AppWindow;
            appWindow.Resize(new SizeInt32(1200, 740));
            appWindow.Title = "Trick Lỏ Pro Vip";

            ThemeService.ThemeChanged += OnThemeChanged;
            ChkDarkMode.IsOn = ThemeService.IsDark;
            UpdateThemeLabel(ThemeService.IsDark);

            // Trì hoãn điều hướng lần đầu tiên cho đến khi cửa sổ đã hiển thị
            this.Activated += OnFirstActivated;
        }

        private void OnFirstActivated(object sender, WindowActivatedEventArgs args)
        {
            // Chỉ chạy một lần
            this.Activated -= OnFirstActivated;
            if (_firstNavigationDone) return;
            _firstNavigationDone = true;

            // Điều hướng với độ ưu tiên thấp để cửa sổ kịp render
            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
            {
                Navigate(typeof(SystemInfoPage));
            });
        }

        private void OnThemeChanged(bool isDark)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                _updatingTheme = true;
                ChkDarkMode.IsOn = isDark;
                UpdateThemeLabel(isDark);
                _updatingTheme = false;
            });
        }

        internal void UpdateThemeLabel(bool isDark)
        {
            TxtThemeIcon.Text = isDark ? "🌙" : "☀";
            TxtThemeLabel.Text = isDark ? "Giao diện tối" : "Giao diện sáng";
        }

        private void DarkMode_Toggled(object sender, RoutedEventArgs e)
        {
            if (_updatingTheme) return;
            bool isDark = ChkDarkMode.IsOn;
            UpdateThemeLabel(isDark);
            ThemeService.Apply(isDark);
        }

        private void SysInfo_Click(object sender, RoutedEventArgs e) => Navigate(typeof(SystemInfoPage));
        private void Deploy_Click(object sender, RoutedEventArgs e) => Navigate(typeof(DeployPage));
        private void Debloat_Click(object sender, RoutedEventArgs e) => Navigate(typeof(DebloatPage));
        private void Optimize_Click(object sender, RoutedEventArgs e) => Navigate(typeof(OptimizePage));
        private void Network_Click(object sender, RoutedEventArgs e) => Navigate(typeof(NetworkFixPage));
        private void Security_Click(object sender, RoutedEventArgs e) => Navigate(typeof(SecurityPage));
        private void QuickSetup_Click(object sender, RoutedEventArgs e) => Navigate(typeof(QuickSetupPage));
        private void BitLocker_Click(object sender, RoutedEventArgs e) => Navigate(typeof(BitLockerPage));
        private void StartupManager_Click(object sender, RoutedEventArgs e) => Navigate(typeof(StartupManagerPage));
        private void DiskCleaner_Click(object sender, RoutedEventArgs e) => Navigate(typeof(DiskCleanerPage));
        private void DriverInfo_Click(object sender, RoutedEventArgs e) => Navigate(typeof(DriverPage));
        private void ProcessManager_Click(object sender, RoutedEventArgs e) => Navigate(typeof(ProcessManagerPage));
        private void BackupRestore_Click(object sender, RoutedEventArgs e) => Navigate(typeof(BackupRestorePage));
        private void ScheduledTask_Click(object sender, RoutedEventArgs e) => Navigate(typeof(ScheduledTaskPage));
        private void WindowsUpdate_Click(object sender, RoutedEventArgs e) => Navigate(typeof(WindowsUpdatePage));
        private void EventLog_Click(object sender, RoutedEventArgs e) => Navigate(typeof(EventLogPage));
        private void Logs_Click(object sender, RoutedEventArgs e) => Navigate(typeof(LogPage));
        private void Settings_Click(object sender, RoutedEventArgs e) => Navigate(typeof(SettingsPage));
        private void OfficeInstall_Click(object sender, RoutedEventArgs e) => Navigate(typeof(OfficeInstallPage));

        private void Navigate(Type pageType)
        {
            MainFrame.Navigate(pageType);
        }
    }
}