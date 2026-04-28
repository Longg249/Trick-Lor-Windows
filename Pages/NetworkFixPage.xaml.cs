using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TrickLor.Services;

namespace TrickLor.Pages
{
    public partial class NetworkFixPage : Page
    {
        public NetworkFixPage()
        {
            InitializeComponent();
            Loaded += (s, e) => LoadDiagnostics();
        }

        // ===== DIAGNOSTICS =====

        private void LoadDiagnostics()
        {
            try
            {
                TxtIP.Text       = GetLocalIP();
                TxtGW.Text       = GetGateway();
                TxtDNS.Text      = GetDNS();
                TxtInternet.Text = CheckInternet() ? "✅ Kết nối OK" : "❌ Không có Internet";
                TxtInternet.Foreground = CheckInternet()
                    ? new SolidColorBrush(Color.FromRgb(34, 197, 94))
                    : new SolidColorBrush(Color.FromRgb(239, 68, 68));
            }
            catch
            {
                TxtIP.Text = TxtGW.Text = TxtDNS.Text = "Không lấy được";
            }
        }

        private static string GetLocalIP()
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("8.8.8.8", 65530);
            return (socket.LocalEndPoint as IPEndPoint)?.Address.ToString() ?? "N/A";
        }

        private static string GetGateway()
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != OperationalStatus.Up) continue;
                foreach (var gw in ni.GetIPProperties().GatewayAddresses)
                    if (gw.Address.AddressFamily == AddressFamily.InterNetwork)
                        return gw.Address.ToString();
            }
            return "N/A";
        }

        private static string GetDNS()
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != OperationalStatus.Up) continue;
                var dnsAddresses = ni.GetIPProperties().DnsAddresses;
                foreach (var dns in dnsAddresses)
                    if (dns.AddressFamily == AddressFamily.InterNetwork)
                        return dns.ToString();
            }
            return "N/A";
        }

        private static bool CheckInternet()
        {
            try
            {
                using var ping = new Ping();
                var reply = ping.Send("8.8.8.8", 2000);
                return reply.Status == IPStatus.Success;
            }
            catch { return false; }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            TxtIP.Text = TxtGW.Text = TxtDNS.Text = "Đang làm mới...";
            LoadDiagnostics();
        }

        // ===== INDIVIDUAL FIXES =====

        private async void FixWinsock_Click(object sender, RoutedEventArgs e)
            => await RunFix(BtnWinsock, StatusWinsock,
                () => NetworkService.ResetWinsockAsync());

        private async void FixTCP_Click(object sender, RoutedEventArgs e)
            => await RunFix(BtnTCP, StatusTCP,
                () => NetworkService.ResetTCPAsync());

        private async void FixDNS_Click(object sender, RoutedEventArgs e)
            => await RunFix(BtnDNS, StatusDNS,
                () => NetworkService.FlushDNSAsync());

        private async void FixDiscovery_Click(object sender, RoutedEventArgs e)
            => await RunFix(BtnDiscovery, StatusDiscovery,
                () => NetworkService.EnableNetworkDiscoveryAsync());

        private async void FixSharing_Click(object sender, RoutedEventArgs e)
            => await RunFix(BtnSharing, StatusSharing,
                () => NetworkService.EnableFileSharingAsync());

        private async void FixSMB_Click(object sender, RoutedEventArgs e)
            => await RunFix(BtnSMB, StatusSMB,
                () => NetworkService.EnableSMBAsync());

        private async void FixServices_Click(object sender, RoutedEventArgs e)
            => await RunFix(BtnServices, StatusServices,
                () => NetworkService.RestartNetworkServicesAsync());

        // ===== FIX ALL =====

        private async void FixAll_Click(object sender, RoutedEventArgs e)
        {
            TxtNetStatus.Text = "⏳ Đang chạy tất cả fix...";
            var progress = new Progress<string>(msg => TxtNetStatus.Text = msg);
            await NetworkService.FixAllAsync(progress);
            TxtNetStatus.Text = "✅ Đã chạy toàn bộ! Khuyến nghị khởi động lại máy.";
            LoadDiagnostics();
            LogService.Add("NetworkFix: Toàn bộ sửa lỗi hoàn tất");
        }

        // ===== HELPER =====

        private async Task RunFix(Button btn, TextBlock status, Func<Task> action)
        {
            btn.IsEnabled = false;
            status.Text = "⏳ Đang chạy...";
            status.Foreground = new SolidColorBrush(Color.FromRgb(245, 158, 11));
            TxtNetStatus.Text = $"⏳ Đang chạy...";

            try
            {
                await action();
                status.Text = "✅ Xong";
                status.Foreground = new SolidColorBrush(Color.FromRgb(34, 197, 94));
            }
            catch (Exception ex)
            {
                status.Text = "❌ Lỗi";
                status.Foreground = new SolidColorBrush(Color.FromRgb(239, 68, 68));
                TxtNetStatus.Text = $"❌ Lỗi: {ex.Message}";
            }
            finally
            {
                btn.IsEnabled = true;
            }
        }
    }
}
