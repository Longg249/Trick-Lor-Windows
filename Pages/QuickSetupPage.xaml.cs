using System;
using System.Windows;
using System.Windows.Controls;
using TrickLor.Services;

namespace TrickLor.Pages
{
    public partial class QuickSetupPage : Page
    {
        public QuickSetupPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ChkThisPC.IsChecked         = QuickSetupService.GetExplorerThisPC();
                ChkClassicMenu.IsChecked    = QuickSetupService.GetClassicContextMenu();
                ChkPhotoViewer.IsChecked    = QuickSetupService.GetPhotoViewer();
                ChkHideTaskbar.IsChecked    = QuickSetupService.GetHideTaskbarIcons();
                ChkAutoBrightness.IsChecked = !QuickSetupService.GetAutoBrightness();

                TxtCurrentUser.Text  = QuickSetupService.GetCurrentUserName();
                TxtAccountType.Text  = QuickSetupService.GetIsCurrentUserAdmin() ? "Administrator" : "Standard User";
                ChkEnableAdmin.IsChecked = QuickSetupService.GetBuiltinAdminEnabled();
            }
            catch
            {
                TxtQuickStatus.Text = "⚠  Không thể đọc trạng thái — chạy với quyền Administrator";
            }
        }

        private async void ApplyQuick_Click(object sender, RoutedEventArgs e)
        {
            BtnApplyQuick.IsEnabled = false;
            TxtQuickStatus.Text = "⏳ Đang áp dụng...";
            int count = 0;

            try
            {
                if (ChkThisPC.IsChecked == true)
                {
                    QuickSetupService.SetExplorerThisPC(true);
                    count++;
                }

                if (ChkClassicMenu.IsChecked == true)
                {
                    QuickSetupService.SetClassicContextMenu(true);
                    count++;
                }

                if (ChkPhotoViewer.IsChecked == true)
                {
                    QuickSetupService.SetPhotoViewer(true);
                    count++;
                }

                if (ChkHideTaskbar.IsChecked == true)
                {
                    QuickSetupService.SetHideTaskbarIcons(true);
                    count++;
                }

                if (ChkAutoBrightness.IsChecked == true)
                {
                    await QuickSetupService.SetAutoBrightnessAsync(false);
                    count++;
                }

                if (ChkRemoveKeyboard.IsChecked == true)
                {
                    TxtQuickStatus.Text = "⏳ Đang xóa bàn phím ngôn ngữ...";
                    await QuickSetupService.RemoveExtraKeyboardsAsync();
                    count++;
                }

                TxtQuickStatus.Text = count == 0
                    ? "⚠  Chưa chọn tùy chọn nào"
                    : $"✅ Đã áp dụng {count} thiết lập — khởi động lại để có hiệu lực đầy đủ";

                if (count > 0)
                    LogService.Add($"Quick Setup: Áp dụng {count} thiết lập nhanh");
            }
            catch (Exception ex)
            {
                TxtQuickStatus.Text = $"❌ Lỗi: {ex.Message}";
            }
            finally
            {
                BtnApplyQuick.IsEnabled = true;
            }
        }

        private async void RenameAccount_Click(object sender, RoutedEventArgs e)
        {
            var newName = TxtNewUsername.Text.Trim();
            if (string.IsNullOrEmpty(newName))
            {
                TxtQuickStatus.Text = "⚠  Vui lòng nhập tên mới cho tài khoản";
                return;
            }

            var currentName = QuickSetupService.GetCurrentUserName();
            if (newName == currentName)
            {
                TxtQuickStatus.Text = "⚠  Tên mới giống với tên hiện tại";
                return;
            }

            TxtQuickStatus.Text = "⏳ Đang đổi tên tài khoản...";
            try
            {
                await QuickSetupService.RenameLocalAccountAsync(currentName, newName);
                TxtCurrentUser.Text = newName;
                TxtNewUsername.Clear();
                TxtQuickStatus.Text = $"✅ Đã đổi tên thành '{newName}' — đăng xuất để có hiệu lực";
                LogService.Add($"Local Account: Đổi tên '{currentName}' → '{newName}'");
            }
            catch (Exception ex)
            {
                TxtQuickStatus.Text = $"❌ Lỗi đổi tên: {ex.Message}";
            }
        }

        private async void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var newPwd     = PwdNew.Password;
            var confirmPwd = PwdConfirm.Password;

            if (string.IsNullOrEmpty(newPwd))
            {
                TxtQuickStatus.Text = "⚠  Vui lòng nhập mật khẩu mới";
                return;
            }
            if (newPwd != confirmPwd)
            {
                TxtQuickStatus.Text = "⚠  Mật khẩu xác nhận không khớp";
                return;
            }

            TxtQuickStatus.Text = "⏳ Đang đổi mật khẩu...";
            try
            {
                var username = QuickSetupService.GetCurrentUserName();
                await QuickSetupService.ChangeLocalPasswordAsync(username, newPwd);
                PwdNew.Clear();
                PwdConfirm.Clear();
                TxtQuickStatus.Text = "✅ Đã đổi mật khẩu thành công";
                LogService.Add($"Local Account: Đổi mật khẩu tài khoản '{username}'");
            }
            catch (Exception ex)
            {
                TxtQuickStatus.Text = $"❌ Lỗi đổi mật khẩu: {ex.Message}";
            }
        }

        private async void ApplyAdminToggle_Click(object sender, RoutedEventArgs e)
        {
            bool enable = ChkEnableAdmin.IsChecked == true;
            TxtQuickStatus.Text = enable
                ? "⏳ Đang kích hoạt tài khoản Administrator..."
                : "⏳ Đang vô hiệu hóa tài khoản Administrator...";
            try
            {
                await QuickSetupService.SetBuiltinAdminAsync(enable);
                TxtQuickStatus.Text = enable
                    ? "✅ Đã kích hoạt tài khoản Administrator"
                    : "✅ Đã vô hiệu hóa tài khoản Administrator";
                LogService.Add($"Local Account: {(enable ? "Kích hoạt" : "Vô hiệu hóa")} tài khoản Administrator");
            }
            catch (Exception ex)
            {
                TxtQuickStatus.Text = $"❌ Lỗi: {ex.Message}";
            }
        }
    }
}
