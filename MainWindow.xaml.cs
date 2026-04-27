using System.Windows;
using WinDeployPro.Pages;
using WinDeployPro.Services;

namespace WinDeployPro
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Navigate(new SystemInfoPage());
        }

        private void SysInfo_Click(object sender, RoutedEventArgs e)    => Navigate(new SystemInfoPage());
        private void Deploy_Click(object sender, RoutedEventArgs e)    => Navigate(new DeployPage());
        private void Optimize_Click(object sender, RoutedEventArgs e)  => Navigate(new OptimizePage());
        private void Network_Click(object sender, RoutedEventArgs e)   => Navigate(new NetworkFixPage());
        private void Security_Click(object sender, RoutedEventArgs e)    => Navigate(new SecurityPage());
        private void QuickSetup_Click(object sender, RoutedEventArgs e) => Navigate(new QuickSetupPage());
        private void BitLocker_Click(object sender, RoutedEventArgs e)  => Navigate(new BitLockerPage());
        private void Logs_Click(object sender, RoutedEventArgs e)      => Navigate(new LogPage());

        private void Navigate(System.Windows.Controls.Page page)
        {
            MainFrame.Navigate(page);
        }
    }
}
