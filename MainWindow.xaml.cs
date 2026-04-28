using System.Windows;
using TrickLor.Pages;
using TrickLor.Services;

namespace TrickLor
{
    public partial class MainWindow : Window
    {
        private bool _updatingTheme;

        public MainWindow()
        {
            InitializeComponent();
            ThemeService.ThemeChanged += OnThemeChanged;
            ChkDarkMode.IsChecked = ThemeService.IsDark;
            UpdateThemeLabel(ThemeService.IsDark);
            Navigate(new SystemInfoPage());
        }

        private void OnThemeChanged(bool isDark)
        {
            Dispatcher.Invoke(() =>
            {
                _updatingTheme = true;
                ChkDarkMode.IsChecked = isDark;
                UpdateThemeLabel(isDark);
                _updatingTheme = false;
            });
        }

        internal void UpdateThemeLabel(bool isDark)
        {
            TxtThemeIcon.Text  = isDark ? "🌙" : "☀";
            TxtThemeLabel.Text = isDark ? "Giao diện tối" : "Giao diện sáng";
        }

        private void DarkMode_Changed(object sender, RoutedEventArgs e)
        {
            if (_updatingTheme) return;
            bool isDark = ChkDarkMode.IsChecked == true;
            UpdateThemeLabel(isDark);
            ThemeService.Apply(isDark);
        }

        private void SysInfo_Click(object sender, RoutedEventArgs e)       => Navigate(new SystemInfoPage());
        private void Deploy_Click(object sender, RoutedEventArgs e)       => Navigate(new DeployPage());
        private void Debloat_Click(object sender, RoutedEventArgs e)      => Navigate(new DebloatPage());
        private void Optimize_Click(object sender, RoutedEventArgs e)     => Navigate(new OptimizePage());
        private void Network_Click(object sender, RoutedEventArgs e)      => Navigate(new NetworkFixPage());
        private void Security_Click(object sender, RoutedEventArgs e)     => Navigate(new SecurityPage());
        private void QuickSetup_Click(object sender, RoutedEventArgs e)   => Navigate(new QuickSetupPage());
        private void BitLocker_Click(object sender, RoutedEventArgs e)    => Navigate(new BitLockerPage());
        private void StartupManager_Click(object sender, RoutedEventArgs e) => Navigate(new StartupManagerPage());
        private void DiskCleaner_Click(object sender, RoutedEventArgs e)  => Navigate(new DiskCleanerPage());
        private void DriverInfo_Click(object sender, RoutedEventArgs e)   => Navigate(new DriverPage());
        private void ProcessManager_Click(object sender, RoutedEventArgs e) => Navigate(new ProcessManagerPage());
        private void BackupRestore_Click(object sender, RoutedEventArgs e) => Navigate(new BackupRestorePage());
        private void ScheduledTask_Click(object sender, RoutedEventArgs e) => Navigate(new ScheduledTaskPage());
        private void WindowsUpdate_Click(object sender, RoutedEventArgs e) => Navigate(new WindowsUpdatePage());
        private void EventLog_Click(object sender, RoutedEventArgs e)     => Navigate(new EventLogPage());
        private void Logs_Click(object sender, RoutedEventArgs e)         => Navigate(new LogPage());
        private void Settings_Click(object sender, RoutedEventArgs e)     => Navigate(new SettingsPage());

        private void Navigate(System.Windows.Controls.Page page)
        {
            MainFrame.Navigate(page);
        }
    }
}
