using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace TrickLor.Services
{
    public record DiskCleanOptions
    {
        public bool WinUpdateCache { get; init; }
        public bool Prefetch       { get; init; }
        public bool WinErrorReport { get; init; }
        public bool ThumbnailCache { get; init; }
        public bool TempFiles      { get; init; }
        public bool RecycleBin     { get; init; }
    }

    public static class DiskCleanerService
    {
        public static async Task<long> EstimateAsync(DiskCleanOptions opts) => await Task.Run(() =>
        {
            long total = 0;
            if (opts.WinUpdateCache) total += FolderSize(@"C:\Windows\SoftwareDistribution\Download");
            if (opts.Prefetch)       total += FolderSize(@"C:\Windows\Prefetch");
            if (opts.WinErrorReport) total += FolderSize(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Microsoft\Windows\WER\ReportQueue"));
            if (opts.ThumbnailCache) total += FolderSize(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Microsoft\Windows\Explorer"));
            if (opts.TempFiles) total += FolderSize(Path.GetTempPath());
            return total;
        });

        public static async Task CleanAsync(DiskCleanOptions opts, IProgress<string>? p = null)
        {
            if (opts.WinUpdateCache)
            {
                p?.Report("⏳ Xoá Windows Update cache...");
                await RunCmd(@"net stop wuauserv /y 2>nul & rd /s /q ""C:\Windows\SoftwareDistribution\Download"" 2>nul & net start wuauserv 2>nul");
                p?.Report("✅ Windows Update cache — Xong");
            }
            if (opts.Prefetch)
            {
                p?.Report("⏳ Xoá Prefetch...");
                await RunCmd(@"del /q /f /s C:\Windows\Prefetch\* 2>nul");
                p?.Report("✅ Prefetch — Xong");
            }
            if (opts.WinErrorReport)
            {
                p?.Report("⏳ Xoá Windows Error Reports...");
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                        @"Microsoft\Windows\WER\ReportQueue");
                await Task.Run(() => DeleteFiles(path));
                p?.Report("✅ Windows Error Reports — Xong");
            }
            if (opts.ThumbnailCache)
            {
                p?.Report("⏳ Xoá Thumbnail cache...");
                await RunCmd(@"del /q /f /s ""%LocalAppData%\Microsoft\Windows\Explorer\thumbcache_*.db"" 2>nul");
                p?.Report("✅ Thumbnail cache — Xong");
            }
            if (opts.TempFiles)
            {
                p?.Report("⏳ Xoá Temp files...");
                await RunCmd(@"del /q /f /s ""%TEMP%\*"" 2>nul & del /q /f /s ""%windir%\Temp\*"" 2>nul");
                p?.Report("✅ Temp Files — Xong");
            }
            if (opts.RecycleBin)
            {
                p?.Report("⏳ Dọn Recycle Bin...");
                await RunCmd(@"PowerShell -Command ""Clear-RecycleBin -Force -ErrorAction SilentlyContinue""");
                p?.Report("✅ Recycle Bin — Xong");
            }
        }

        public static string FormatSize(long bytes)
        {
            if (bytes < 1024)          return $"{bytes} B";
            if (bytes < 1_048_576)     return $"{bytes / 1024.0:F1} KB";
            if (bytes < 1_073_741_824) return $"{bytes / 1_048_576.0:F1} MB";
            return $"{bytes / 1_073_741_824.0:F2} GB";
        }

        private static long FolderSize(string path)
        {
            try
            {
                if (!Directory.Exists(path)) return 0;
                long size = 0;
                foreach (var f in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                    try { size += new FileInfo(f).Length; } catch { }
                return size;
            }
            catch { return 0; }
        }

        private static void DeleteFiles(string path)
        {
            try
            {
                if (!Directory.Exists(path)) return;
                foreach (var f in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                    try { File.Delete(f); } catch { }
            }
            catch { }
        }

        private static Task RunCmd(string cmd) => Task.Run(() =>
        {
            var psi = new ProcessStartInfo("cmd.exe", $"/c {cmd}") { CreateNoWindow = true, UseShellExecute = false };
            using var proc = Process.Start(psi);
            proc?.WaitForExit();
        });
    }
}
