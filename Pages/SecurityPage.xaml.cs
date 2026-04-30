using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TrickLor.Services;

namespace TrickLor.Pages
{
    public sealed partial class SecurityPage : Page
    {
        public SecurityPage() => InitializeComponent();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ChkToast.IsChecked    = SecurityService.GetToast();
                ChkUAC.IsChecked      = SecurityService.GetUAC();
                ChkSmart.IsChecked    = SecurityService.GetSmartScreen();
                ChkAlerts.IsChecked   = SecurityService.GetSecurityAlerts();
                ChkFirewall.IsChecked = SecurityService.GetFirewall();
            }
            catch { TxtSecStatus.Text = "⚠  Không thể đọc trạng thái — chạy với quyền Administrator"; }
        }

        private async void ApplySecurity_Click(object sender, RoutedEventArgs e)
        {
            BtnApplySecurity.IsEnabled = false;
            TxtSecStatus.Text = "⏳ Đang áp dụng...";
            try
            {
                SecurityService.SetToast(ChkToast.IsChecked == true);
                SecurityService.SetUAC(ChkUAC.IsChecked == true);
                SecurityService.SetSmartScreen(ChkSmart.IsChecked == true);
                SecurityService.SetSecurityAlerts(ChkAlerts.IsChecked == true);
                await SecurityService.SetFirewallAsync(ChkFirewall.IsChecked == true);
                TxtSecStatus.Text = "✅ Đã áp dụng — một số thay đổi có hiệu lực sau khi khởi động lại";
                LogService.Add("Security: Đã cấu hình Windows Security");
            }
            catch (Exception ex) { TxtSecStatus.Text = $"❌ Lỗi: {ex.Message}"; }
            finally { BtnApplySecurity.IsEnabled = true; }
        }
    }
}
