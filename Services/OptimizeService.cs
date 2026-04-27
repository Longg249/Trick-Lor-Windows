using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WinDeployPro.Services
{
    // ===== OPTIONS STRUCTS =====
    public record OptimizeOptions
    {
        public bool VisualFX    { get; init; }
        public bool PowerPlan   { get; init; }
        public bool SysMain     { get; init; }
        public bool SearchIndex { get; init; }
    }

    public record PrivacyOptions
    {
        public bool Telemetry       { get; init; }
        public bool Cortana         { get; init; }
        public bool AdvertisingId   { get; init; }
        public bool ActivityHistory { get; init; }
    }

    public record StorageOptions
    {
        public bool TempFiles   { get; init; }
        public bool RecycleBin  { get; init; }
        public bool Hibernation { get; init; }
    }

    public record StartupOptions
    {
        public bool FastBoot  { get; init; }
        public bool GameBar   { get; init; }
        public bool FlushDns  { get; init; }
    }

    // ===== SERVICE =====
    public static class OptimizeService
    {
        public static async Task ApplyPerformanceAsync(OptimizeOptions opts, IProgress<string>? p = null)
        {
            if (opts.VisualFX)
            {
                p?.Report("⏳ Tắt hiệu ứng hình ảnh...");
                await Run(@"reg add ""HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects"" /v VisualFXSetting /t REG_DWORD /d 2 /f");
                p?.Report("✅ Visual FX — Xong");
            }
            if (opts.PowerPlan)
            {
                p?.Report("⏳ Đặt High Performance Power Plan...");
                await Run("powercfg /setactive 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c");
                p?.Report("✅ Power Plan — Xong");
            }
            if (opts.SysMain)
            {
                p?.Report("⏳ Tắt SysMain...");
                await Run("sc config SysMain start= disabled & net stop SysMain");
                p?.Report("✅ SysMain — Xong");
            }
            if (opts.SearchIndex)
            {
                p?.Report("⏳ Tắt Windows Search Indexing...");
                await Run("sc config WSearch start= disabled & net stop WSearch");
                p?.Report("✅ Search Index — Xong");
            }
        }

        public static async Task ApplyPrivacyAsync(PrivacyOptions opts, IProgress<string>? p = null)
        {
            if (opts.Telemetry)
            {
                p?.Report("⏳ Tắt Telemetry...");
                await Run(@"reg add ""HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection"" /v AllowTelemetry /t REG_DWORD /d 0 /f");
                p?.Report("✅ Telemetry — Xong");
            }
            if (opts.Cortana)
            {
                p?.Report("⏳ Tắt Cortana...");
                await Run(@"reg add ""HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Search"" /v AllowCortana /t REG_DWORD /d 0 /f");
                p?.Report("✅ Cortana — Xong");
            }
            if (opts.AdvertisingId)
            {
                p?.Report("⏳ Tắt Advertising ID...");
                await Run(@"reg add ""HKCU\Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo"" /v Enabled /t REG_DWORD /d 0 /f");
                p?.Report("✅ Advertising ID — Xong");
            }
            if (opts.ActivityHistory)
            {
                p?.Report("⏳ Tắt Activity History...");
                await Run(@"reg add ""HKLM\SOFTWARE\Policies\Microsoft\Windows\System"" /v PublishUserActivities /t REG_DWORD /d 0 /f");
                p?.Report("✅ Activity History — Xong");
            }
        }

        public static async Task ApplyStorageAsync(StorageOptions opts, IProgress<string>? p = null)
        {
            if (opts.TempFiles)
            {
                p?.Report("⏳ Xoá Temp files...");
                await Run(@"cmd /c del /q /f /s ""%TEMP%\*"" 2>nul & del /q /f /s ""%windir%\Temp\*"" 2>nul");
                p?.Report("✅ Temp Files — Xong");
            }
            if (opts.RecycleBin)
            {
                p?.Report("⏳ Dọn Recycle Bin...");
                await Run(@"PowerShell -Command ""Clear-RecycleBin -Force -ErrorAction SilentlyContinue""");
                p?.Report("✅ Recycle Bin — Xong");
            }
            if (opts.Hibernation)
            {
                p?.Report("⏳ Tắt Hibernation...");
                await Run("powercfg /hibernate off");
                p?.Report("✅ Hibernation — Xong");
            }
        }

        public static async Task ApplyStartupAsync(StartupOptions opts, IProgress<string>? p = null)
        {
            if (opts.FastBoot)
            {
                p?.Report("⏳ Bật Fast Startup...");
                await Run(@"reg add ""HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Power"" /v HiberbootEnabled /t REG_DWORD /d 1 /f");
                p?.Report("✅ Fast Startup — Xong");
            }
            if (opts.GameBar)
            {
                p?.Report("⏳ Tắt Xbox Game Bar...");
                await Run(@"reg add ""HKCU\Software\Microsoft\Windows\CurrentVersion\GameDVR"" /v AppCaptureEnabled /t REG_DWORD /d 0 /f");
                p?.Report("✅ Game Bar — Xong");
            }
            if (opts.FlushDns)
            {
                p?.Report("⏳ Flush DNS...");
                await Run("ipconfig /flushdns");
                p?.Report("✅ DNS Flushed — Xong");
            }
        }

        private static Task Run(string cmd) => Task.Run(() =>
        {
            var psi = new ProcessStartInfo("cmd.exe", $"/c {cmd}")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
            };
            using var proc = Process.Start(psi) ?? throw new Exception("Không thể chạy lệnh.");
            proc.WaitForExit();
        });
    }
}
