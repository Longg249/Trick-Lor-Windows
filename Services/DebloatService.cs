using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WinDeployPro.Services
{
    public static class DebloatService
    {
        // ── System Restore ───────────────────────────────────────────────────

        public static async Task<string> CreateRestorePointAsync(IProgress<string> progress)
        {
            try
            {
                progress.Report("⏳ Đang bật Volume Shadow Copy...");
                await RunCmdAsync("sc config VSS start= auto");
                await RunCmdAsync("net start VSS 2>nul");

                progress.Report("⏳ Đang bật System Restore trên ổ C:\\...");
                await RunPSAsync(@"Enable-ComputerRestore -Drive 'C:\' -ErrorAction SilentlyContinue");

                progress.Report("⏳ Đang tạo điểm khôi phục — vui lòng chờ...");
                var out_ = await RunPSWithOutputAsync(@"
Checkpoint-Computer -Description 'TrickLor Pre-Debloat' -RestorePointType MODIFY_SETTINGS -ErrorAction SilentlyContinue
Write-Output 'DONE'
");
                return out_.Contains("DONE")
                    ? "✅ Đã tạo điểm khôi phục thành công"
                    : "⚠  Windows giới hạn 1 điểm / 24h — hệ thống đã có điểm gần đây";
            }
            catch (Exception ex)
            {
                return $"❌ Lỗi tạo điểm khôi phục: {ex.Message}";
            }
        }

        public static void OpenSystemRestoreUI()
        {
            try { Process.Start(new ProcessStartInfo("rstrui.exe") { UseShellExecute = true }); }
            catch { }
        }

        // ── AppX removal (current user + all users + provisioned) ────────────

        public static Task RemoveAppxAsync(string pattern, IProgress<string> progress)
        {
            progress.Report($"⏳ Gỡ {pattern}...");
            return RunPSAsync($@"
$p = '{pattern}'
Get-AppxPackage         -Name $p -ErrorAction SilentlyContinue | Remove-AppxPackage            -ErrorAction SilentlyContinue
Get-AppxPackage -AllUsers -Name $p -ErrorAction SilentlyContinue | Remove-AppxPackage           -ErrorAction SilentlyContinue
Get-AppxProvisionedPackage -Online -ErrorAction SilentlyContinue |
    Where-Object DisplayName -Like $p |
    Remove-AppxProvisionedPackage -Online -ErrorAction SilentlyContinue
");
        }

        // ── OneDrive ─────────────────────────────────────────────────────────

        public static async Task RemoveOneDriveAsync(IProgress<string> progress)
        {
            progress.Report("⏳ Đang gỡ OneDrive...");
            await RunCmdAsync("taskkill /f /im OneDrive.exe 2>nul");
            await Task.Delay(1500);
            await RunPSAsync(@"
$paths = @(
    ""$env:SystemRoot\SysWOW64\OneDriveSetup.exe"",
    ""$env:SystemRoot\System32\OneDriveSetup.exe"",
    ""$env:LOCALAPPDATA\Microsoft\OneDrive\OneDriveSetup.exe""
)
foreach ($p in $paths) {
    if (Test-Path $p) {
        Start-Process $p -ArgumentList '/uninstall' -Wait -ErrorAction SilentlyContinue
        break
    }
}
# Remove registry autorun
Remove-ItemProperty -Path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Run' -Name 'OneDrive' -ErrorAction SilentlyContinue
");
        }

        // ── Registry / feature disables ──────────────────────────────────────

        public static Task DisableChatWidgetAsync(IProgress<string> progress)
        {
            progress.Report("⏳ Tắt Teams Chat Taskbar...");
            return RunPSAsync(@"
$p = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced'
Set-ItemProperty -Path $p -Name TaskbarMn -Value 0 -Type DWord -Force -ErrorAction SilentlyContinue
");
        }

        public static Task DisableWidgetsAsync(IProgress<string> progress)
        {
            progress.Report("⏳ Tắt Widgets Taskbar...");
            return RunPSAsync(@"
$p = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced'
Set-ItemProperty -Path $p -Name TaskbarDa -Value 0 -Type DWord -Force -ErrorAction SilentlyContinue
");
        }

        public static Task DisableBingSearchAsync(IProgress<string> progress)
        {
            progress.Report("⏳ Tắt Bing Search trong Start...");
            return RunPSAsync(@"
$p = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Search'
if (-not (Test-Path $p)) { New-Item -Path $p -Force | Out-Null }
Set-ItemProperty -Path $p -Name BingSearchEnabled -Value 0 -Type DWord -Force -ErrorAction SilentlyContinue
Set-ItemProperty -Path $p -Name CortanaConsent    -Value 0 -Type DWord -Force -ErrorAction SilentlyContinue
");
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private static Task RunCmdAsync(string cmd) => Task.Run(() =>
        {
            using var p = Process.Start(new ProcessStartInfo("cmd.exe", $"/c {cmd}")
            { CreateNoWindow = true, UseShellExecute = false });
            p?.WaitForExit();
        });

        private static Task RunPSAsync(string script) => Task.Run(() =>
        {
            var enc = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(script));
            using var p = Process.Start(new ProcessStartInfo("powershell.exe",
                $"-NonInteractive -NoProfile -ExecutionPolicy Bypass -EncodedCommand {enc}")
            { CreateNoWindow = true, UseShellExecute = false });
            p?.WaitForExit();
        });

        private static Task<string> RunPSWithOutputAsync(string script) => Task.Run(() =>
        {
            var enc = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(script));
            var psi = new ProcessStartInfo("powershell.exe",
                $"-NonInteractive -NoProfile -ExecutionPolicy Bypass -EncodedCommand {enc}")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            using var p = Process.Start(psi);
            var output = p?.StandardOutput.ReadToEnd() ?? "";
            p?.WaitForExit();
            return output;
        });
    }
}
