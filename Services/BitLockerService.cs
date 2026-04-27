using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WinDeployPro.Services
{
    public class DriveInfo
    {
        public string Letter { get; set; } = "";
        public string Status { get; set; } = "";   // Encrypted / Decrypted / EncryptionInProgress
        public string Protection { get; set; } = "";   // On / Off
        public int Percentage { get; set; } = 0;
        public bool IsEncrypted => Status == "FullyEncrypted";
        public bool IsDecrypted => Status == "FullyDecrypted";
    }

    public static class BitLockerService
    {
        // ===== LẤY DANH SÁCH Ổ ĐĨA + TRẠNG THÁI =====
        public static async Task<List<DriveInfo>> GetDrivesAsync()
        {
            var result = new List<DriveInfo>();

            // Dùng manage-bde để lấy thông tin từng ổ
            var output = await RunAsync("manage-bde -status");

            // Parse output thủ công
            DriveInfo? current = null;
            foreach (var line in output.Split('\n'))
            {
                var trimmed = line.Trim();

                // Phát hiện ổ đĩa mới: "Volume C:"
                if (trimmed.StartsWith("Volume") && trimmed.Contains(":"))
                {
                    if (current != null) result.Add(current);
                    current = new DriveInfo
                    {
                        Letter = trimmed.Split(' ')[1].TrimEnd(':')
                    };
                }

                if (current == null) continue;

                if (trimmed.StartsWith("Conversion Status:"))
                    current.Status = trimmed.Split(':')[1].Trim().Replace(" ", "");

                if (trimmed.StartsWith("Protection Status:"))
                    current.Protection = trimmed.Split(':')[1].Trim();

                if (trimmed.StartsWith("Percentage Encrypted:"))
                {
                    var pct = trimmed.Split(':')[1].Trim().Replace("%", "");
                    int.TryParse(pct, out var val);
                    current.Percentage = val;
                }
            }

            if (current != null) result.Add(current);

            return result;
        }

        // ===== MÃ HOÁ Ổ ĐĨA =====
        public static async Task EncryptAsync(string driveLetter, IProgress<string>? p = null)
        {
            p?.Report($"⏳ Đang bật BitLocker cho ổ {driveLetter}:...");

            // Bật BitLocker với TPM (không cần password)
            // Nếu máy không có TPM, dùng -rp để tạo recovery password
            await RunAsync($"manage-bde -on {driveLetter}: -rp");

            p?.Report($"✅ Đã bật BitLocker cho ổ {driveLetter}:");
            p?.Report($"⚠  Lưu Recovery Key ở nơi an toàn!");
        }

        // ===== GIẢI MÃ Ổ ĐĨA =====
        public static async Task DecryptAsync(string driveLetter, IProgress<string>? p = null)
        {
            p?.Report($"⏳ Đang tắt BitLocker cho ổ {driveLetter}:...");
            await RunAsync($"manage-bde -off {driveLetter}:");
            p?.Report($"✅ Đã bắt đầu giải mã ổ {driveLetter}: (có thể mất vài phút)");
        }

        // ===== LẤY RECOVERY KEY =====
        public static async Task<string> GetRecoveryKeyAsync(string driveLetter)
        {
            var output = await RunAsync($"manage-bde -protectors -get {driveLetter}:");
            return output;
        }

        // ===== BACKUP RECOVERY KEY LÊN AD / FILE =====
        public static async Task BackupRecoveryKeyAsync(string driveLetter, string outputPath, IProgress<string>? p = null)
        {
            p?.Report("⏳ Đang xuất Recovery Key...");
            await RunAsync($"manage-bde -protectors -get {driveLetter}: > \"{outputPath}\"");
            p?.Report($"✅ Đã lưu Recovery Key tại: {outputPath}");
        }

        // ===== SUSPEND (tạm dừng, dùng khi update BIOS) =====
        public static async Task SuspendAsync(string driveLetter, IProgress<string>? p = null)
        {
            p?.Report($"⏳ Tạm dừng BitLocker ổ {driveLetter}:...");
            await RunAsync($"manage-bde -protectors -disable {driveLetter}:");
            p?.Report($"✅ BitLocker ổ {driveLetter}: đã tạm dừng");
        }

        // ===== RESUME =====
        public static async Task ResumeAsync(string driveLetter, IProgress<string>? p = null)
        {
            p?.Report($"⏳ Khôi phục BitLocker ổ {driveLetter}:...");
            await RunAsync($"manage-bde -protectors -enable {driveLetter}:");
            p?.Report($"✅ BitLocker ổ {driveLetter}: đã khôi phục");
        }

        // ===== HELPER =====
        private static Task<string> RunAsync(string command) => Task.Run(() =>
        {
            var psi = new ProcessStartInfo("cmd.exe", $"/c {command}")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            using var proc = Process.Start(psi)
                ?? throw new Exception("Không thể chạy lệnh.");
            var output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            return output;
        });
    }
}