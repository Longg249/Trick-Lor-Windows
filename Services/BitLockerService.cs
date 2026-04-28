using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TrickLor.Services
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

            // Get-BitLockerVolume trả về object có tên field cố định, không phụ thuộc locale Windows
            var output = await RunPSAsync(@"
Get-BitLockerVolume -ErrorAction SilentlyContinue | ForEach-Object {
    $letter = $_.MountPoint.TrimEnd('\').TrimEnd(':')
    $status = $_.VolumeStatus.ToString()
    $prot   = $_.ProtectionStatus.ToString()
    $pct    = [int]$_.EncryptionPercentage
    ""$letter|$status|$prot|$pct""
}
");
            foreach (var line in output.Split('\n'))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;
                var parts = trimmed.Split('|');
                if (parts.Length < 4) continue;

                var letter = parts[0].Trim();
                if (string.IsNullOrEmpty(letter)) continue;

                // Chuẩn hoá VolumeStatus → FullyEncrypted / FullyDecrypted / (in-progress)
                var rawStatus = parts[1].Trim();
                string status;
                if (rawStatus.Equals("FullyEncrypted", StringComparison.OrdinalIgnoreCase) ||
                    rawStatus.Equals("FullyEncryptedWipeOnly", StringComparison.OrdinalIgnoreCase))
                    status = "FullyEncrypted";
                else if (rawStatus.Equals("FullyDecrypted", StringComparison.OrdinalIgnoreCase))
                    status = "FullyDecrypted";
                else
                    status = rawStatus; // EncryptionInProgress, DecryptionInProgress, etc.

                var protection = parts[2].Trim() == "On" ? "Protection On" : "Protection Off";
                int.TryParse(parts[3].Trim(), out var pct);

                result.Add(new DriveInfo
                {
                    Letter = letter,
                    Status = status,
                    Protection = protection,
                    Percentage = pct
                });
            }

            // fallback: nếu Get-BitLockerVolume không trả gì (Home edition không có BitLocker)
            // thì liệt kê các ổ đĩa fixed từ System.IO và đánh dấu Decrypted
            if (result.Count == 0)
            {
                foreach (var d in System.IO.DriveInfo.GetDrives())
                {
                    if (d.DriveType != System.IO.DriveType.Fixed) continue;
                    result.Add(new DriveInfo
                    {
                        Letter = d.Name.Substring(0, 1),
                        Status = "FullyDecrypted",
                        Protection = "Protection Off",
                        Percentage = 0
                    });
                }
            }

            return result;
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
            var escaped = outputPath.Replace("'", "''");
            await RunPSAsync($@"
manage-bde -protectors -get {driveLetter}: | Out-File -FilePath '{escaped}' -Encoding UTF8
");
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

        // ===== HELPERS =====
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

        private static Task<string> RunPSAsync(string script) => Task.Run(() =>
        {
            var enc = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(script));
            var psi = new ProcessStartInfo("powershell.exe",
                $"-NonInteractive -NoProfile -ExecutionPolicy Bypass -EncodedCommand {enc}")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            using var proc = Process.Start(psi)
                ?? throw new Exception("Không thể chạy PowerShell.");
            var output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            return output;
        });
    }
}