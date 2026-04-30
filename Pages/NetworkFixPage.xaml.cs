using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using TrickLor.Services;
using Windows.UI;

namespace TrickLor.Pages
{
    public sealed partial class NetworkFixPage : Page
    {
        public NetworkFixPage()
        {
            InitializeComponent();
            Loaded += (s, e) => LoadDiagnostics();
        }

        private void LoadDiagnostics()
        {
            try
            {
                TxtIP.Text = GetLocalIP();
                TxtGW.Text = GetGateway();
                TxtDNS.Text = GetDNS();
                bool ok = CheckInternet();
                TxtInternet.Text = ok ? "✅ Kết nối OK" : "❌ Không có Internet";
                TxtInternet.Foreground = ok
                    ? new SolidColorBrush(Color.FromArgb(255, 34, 197, 94))
                    : new SolidColorBrush(Color.FromArgb(255, 239, 68, 68));

                // Đọc trạng thái mặc định cho hai tùy chọn bảo mật
                ChkInsecureLogon.IsChecked = NetworkService.GetInsecureLogonEnabled();
                ChkDigitalSign.IsChecked = NetworkService.GetDigitalSignEnabled();
            }
            catch
            {
                TxtIP.Text = TxtGW.Text = TxtDNS.Text = "Không lấy được";
            }
        }

        private static string GetLocalIP()
        {
            using var socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
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
                foreach (var dns in ni.GetIPProperties().DnsAddresses)
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

        private async void FixWinsock_Click(object sender, RoutedEventArgs e)
            => await RunFix(BtnWinsock, StatusWinsock, () => NetworkService.ResetWinsockAsync());

        private async void FixTCP_Click(object sender, RoutedEventArgs e)
            => await RunFix(BtnTCP, StatusTCP, () => NetworkService.ResetTCPAsync());

        private async void FixDNS_Click(object sender, RoutedEventArgs e)
            => await RunFix(BtnDNS, StatusDNS, () => NetworkService.FlushDNSAsync());

        private async void FixDiscovery_Click(object sender, RoutedEventArgs e)
            => await RunFix(BtnDiscovery, StatusDiscovery, () => NetworkService.EnableNetworkDiscoveryAsync());

        private async void FixSharing_Click(object sender, RoutedEventArgs e)
            => await RunFix(BtnSharing, StatusSharing, () => NetworkService.EnableFileSharingAsync());

        private async void FixSMB_Click(object sender, RoutedEventArgs e)
            => await RunFix(BtnSMB, StatusSMB, () => NetworkService.EnableSMBAsync());

        private async void FixServices_Click(object sender, RoutedEventArgs e)
            => await RunFix(BtnServices, StatusServices, () => NetworkService.RestartNetworkServicesAsync());

        private async void FixAll_Click(object sender, RoutedEventArgs e)
        {
            TxtNetStatus.Text = "⏳ Đang chạy tất cả fix...";
            var progress = new Progress<string>(msg => TxtNetStatus.Text = msg);
            await NetworkService.FixAllAsync(progress);
            TxtNetStatus.Text = "✅ Đã chạy toàn bộ! Khuyến nghị khởi động lại máy.";
            LoadDiagnostics();
            LogService.Add("NetworkFix: Toàn bộ sửa lỗi hoàn tất");
        }

        private async Task RunFix(Button btn, TextBlock status, Func<Task> action)
        {
            btn.IsEnabled = false;
            status.Text = "⏳ Đang chạy...";
            status.Foreground = new SolidColorBrush(Color.FromArgb(255, 245, 158, 11));
            TxtNetStatus.Text = "⏳ Đang chạy...";

            try
            {
                await action();
                status.Text = "✅ Xong";
                status.Foreground = new SolidColorBrush(Color.FromArgb(255, 34, 197, 94));
            }
            catch (Exception ex)
            {
                status.Text = "❌ Lỗi";
                status.Foreground = new SolidColorBrush(Color.FromArgb(255, 239, 68, 68));
                TxtNetStatus.Text = $"❌ Lỗi: {ex.Message}";
            }
            finally
            {
                btn.IsEnabled = true;
            }
        }

        // ===== HAI TÙY CHỌN BẢO MẬT MẠNG MỚI =====
        private async void ApplyInsecureLogon_Click(object sender, RoutedEventArgs e)
        {
            BtnApplyInsecure.IsEnabled = false;
            bool enable = ChkInsecureLogon.IsChecked == true;
            try
            {
                await NetworkService.SetInsecureLogonAsync(enable);
                TxtNetStatus.Text = enable ?
                    "✅ Đã bật Allow insecure logon." :
                    "✅ Đã tắt Allow insecure logon.";
                LogService.Add($"NetworkFix: Allow insecure logon = {enable}");
            }
            catch (Exception ex)
            {
                TxtNetStatus.Text = $"❌ Lỗi: {ex.Message}";
            }
            finally
            {
                BtnApplyInsecure.IsEnabled = true;
            }
        }

        private async void ApplyDigitalSign_Click(object sender, RoutedEventArgs e)
        {
            BtnApplyDigitalSign.IsEnabled = false;
            bool enable = ChkDigitalSign.IsChecked == true;
            try
            {
                await NetworkService.SetDigitalSignCommunicationAsync(enable);
                TxtNetStatus.Text = enable ?
                    "✅ Đã bật Digital sign communication (always)." :
                    "✅ Đã tắt Digital sign communication.";
                LogService.Add($"NetworkFix: Digital sign communication (always) = {enable}");
            }
            catch (Exception ex)
            {
                TxtNetStatus.Text = $"❌ Lỗi: {ex.Message}";
            }
            finally
            {
                BtnApplyDigitalSign.IsEnabled = true;
            }
        }
    }
}