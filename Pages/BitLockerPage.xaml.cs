using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using WinDeployPro.Services;

namespace WinDeployPro.Pages
{
    // ViewModel cho từng ổ đĩa
    public class DriveViewModel
    {
        public string Letter { get; set; } = "";
        public string Icon { get; set; } = "💾";
        public string StatusLabel { get; set; } = "";
        public string StatusBg { get; set; } = "#1A1A1A";
        public string StatusFg { get; set; } = "#94A3B8";
        public int Percentage { get; set; } = 0;
        public string ProgressLabel { get; set; } = "";
        public string ProgressColor { get; set; } = "#3B82F6";
        public string ActionLabel { get; set; } = "";
        public string ActionStyle { get; set; } = "";
        public string SuspendLabel { get; set; } = "";
        public Visibility SuspendVisible { get; set; } = Visibility.Collapsed;
        public Visibility EncryptedVisible { get; set; } = Visibility.Collapsed;
    }

    public partial class BitLockerPage : Page
    {
        public BitLockerPage()
        {
            InitializeComponent();
            Loaded += (s, e) => _ = LoadDrivesAsync();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
            => _ = LoadDrivesAsync();

        // ===== LOAD DRIVES =====
        private async System.Threading.Tasks.Task LoadDrivesAsync()
        {
            TxtStatus.Text = "⏳ Đang kiểm tra BitLocker...";
            LogLine("Đang quét ổ đĩa...");

            try
            {
                var drives = await BitLockerService.GetDrivesAsync();
                var vms = new List<DriveViewModel>();

                foreach (var d in drives)
                {
                    var vm = new DriveViewModel { Letter = $"Ổ {d.Letter}:" };

                    switch (d.Status)
                    {
                        case "FullyEncrypted":
                            vm.StatusLabel = "● Đã mã hoá";
                            vm.StatusBg = "#0D2B18";
                            vm.StatusFg = "#22C55E";
                            vm.ProgressColor = "#22C55E";
                            vm.Percentage = 100;
                            vm.ProgressLabel = "100% — BitLocker đang bảo vệ";
                            vm.Icon = "🔒";
                            vm.ActionLabel = "🔓  Giải mã";
                            vm.ActionStyle = "BtnRed";
                            vm.SuspendLabel = d.Protection == "Protection On"
                                               ? "⏸  Tạm dừng" : "▶  Tiếp tục";
                            vm.SuspendVisible = Visibility.Visible;
                            vm.EncryptedVisible = Visibility.Visible;
                            break;

                        case "FullyDecrypted":
                            vm.StatusLabel = "○ Chưa mã hoá";
                            vm.StatusBg = "#2A0A00";
                            vm.StatusFg = "#EF4444";
                            vm.ProgressColor = "#EF4444";
                            vm.Percentage = 0;
                            vm.ProgressLabel = "0% — Không được bảo vệ";
                            vm.Icon = "🔓";
                            vm.ActionLabel = "🔒  Mã hoá";
                            vm.ActionStyle = "BtnGreen";
                            vm.SuspendVisible = Visibility.Collapsed;
                            vm.EncryptedVisible = Visibility.Collapsed;
                            break;

                        default: // đang trong quá trình
                            vm.StatusLabel = "⟳ Đang xử lý";
                            vm.StatusBg = "#1A1A00";
                            vm.StatusFg = "#F59E0B";
                            vm.ProgressColor = "#F59E0B";
                            vm.Percentage = d.Percentage;
                            vm.ProgressLabel = $"{d.Percentage}% — Đang tiến hành...";
                            vm.Icon = "⏳";
                            vm.ActionLabel = "⏳  Đang xử lý";
                            vm.ActionStyle = "BtnGhost";
                            break;
                    }

                    vms.Add(vm);
                }

                DriveList.ItemsSource = vms;
                TxtStatus.Text = $"✅ Tìm thấy {vms.Count} ổ đĩa";
                LogLine($"Quét xong: {vms.Count} ổ đĩa");
            }
            catch (Exception ex)
            {
                TxtStatus.Text = $"❌ Lỗi: {ex.Message}";
                LogLine($"❌ {ex.Message}");
            }
        }

        // ===== ENCRYPT / DECRYPT =====
        private async void Action_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            var letter = btn.Tag?.ToString()?.Replace("Ổ ", "").Replace(":", "") ?? "";

            // Xác nhận trước khi thực hiện
            var action = btn.Content.ToString()!.Contains("Mã hoá") ? "mã hoá" : "giải mã";
            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn {action} ổ {letter}:?\n\n" +
                (action == "mã hoá"
                    ? "⚠  Hãy lưu Recovery Key sau khi mã hoá!"
                    : "⚠  Dữ liệu sẽ không được bảo vệ sau khi giải mã."),
                $"Xác nhận {action}",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            btn.IsEnabled = false;
            var progress = new Progress<string>(LogLine);

            try
            {
                if (action == "mã hoá")
                    await BitLockerService.EncryptAsync(letter, progress);
                else
                    await BitLockerService.DecryptAsync(letter, progress);

                LogService.Add($"BitLocker: {action} ổ {letter}:");
            }
            catch (Exception ex)
            {
                LogLine($"❌ Lỗi: {ex.Message}");
            }
            finally
            {
                btn.IsEnabled = true;
                await LoadDrivesAsync();
            }
        }

        // ===== SUSPEND / RESUME =====
        private async void Suspend_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            var letter = btn.Tag?.ToString()?.Replace("Ổ ", "").Replace(":", "") ?? "";
            var isSuspend = btn.Content.ToString()!.Contains("Tạm dừng");

            btn.IsEnabled = false;
            var progress = new Progress<string>(LogLine);

            try
            {
                if (isSuspend)
                    await BitLockerService.SuspendAsync(letter, progress);
                else
                    await BitLockerService.ResumeAsync(letter, progress);

                LogService.Add($"BitLocker: {(isSuspend ? "Tạm dừng" : "Tiếp tục")} ổ {letter}:");
            }
            catch (Exception ex) { LogLine($"❌ {ex.Message}"); }
            finally
            {
                btn.IsEnabled = true;
                await LoadDrivesAsync();
            }
        }

        // ===== BACKUP RECOVERY KEY =====
        private async void BackupKey_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            var letter = btn.Tag?.ToString()?.Replace("Ổ ", "").Replace(":", "") ?? "";

            var dlg = new SaveFileDialog
            {
                Title = $"Lưu Recovery Key ổ {letter}:",
                FileName = $"BitLocker_RecoveryKey_{letter}",
                DefaultExt = ".txt",
                Filter = "Text files (*.txt)|*.txt"
            };

            if (dlg.ShowDialog() != true) return;

            var progress = new Progress<string>(LogLine);
            await BitLockerService.BackupRecoveryKeyAsync(letter, dlg.FileName, progress);
            LogService.Add($"BitLocker: Xuất Recovery Key ổ {letter}: → {dlg.FileName}");
        }

        // ===== LOG HELPER =====
        private void LogLine(string msg)
        {
            var time = DateTime.Now.ToString("HH:mm:ss");
            LogBox.Text += $"[{time}] {msg}\n";
            LogScroll.ScrollToBottom();
        }
    }
}