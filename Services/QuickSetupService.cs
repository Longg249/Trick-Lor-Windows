using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TrickLor.Services
{
    public static class QuickSetupService
    {
        // ── 1. Explorer mở This PC ───────────────────────────────────────────
        public static bool GetExplorerThisPC()
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced");
            return key?.GetValue("LaunchTo") is 1;
        }

        public static void SetExplorerThisPC(bool enable)
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", writable: true);
            key?.SetValue("LaunchTo", enable ? 1 : 2, RegistryValueKind.DWord);
        }

        // ── 2. Context Menu cổ điển (Win10 style) ───────────────────────────
        private const string ClassicMenuKey =
            @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32";

        public static bool GetClassicContextMenu()
        {
            using var key = Registry.CurrentUser.OpenSubKey(ClassicMenuKey);
            return key != null;
        }

        public static void SetClassicContextMenu(bool enable)
        {
            if (enable)
            {
                using var key = Registry.CurrentUser.CreateSubKey(ClassicMenuKey);
                key.SetValue("", "", RegistryValueKind.String);
            }
            else
            {
                Registry.CurrentUser.DeleteSubKeyTree(
                    @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}", throwOnMissingSubKey: false);
            }
        }

        // ── 3. Windows Photo Viewer ──────────────────────────────────────────
        public static bool GetPhotoViewer()
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Classes\.jpg\OpenWithProgids");
            return key?.GetValue("PhotoViewer.FileAssoc.Jpeg") != null;
        }

        public static void SetPhotoViewer(bool enable)
        {
            string[] exts = { ".bmp", ".dib", ".gif", ".jfif", ".jpe", ".jpeg", ".jpg", ".png", ".tif", ".tiff", ".wdp" };
            string[] progids = { "PhotoViewer.FileAssoc.Bitmap", "PhotoViewer.FileAssoc.Gif",
                                  "PhotoViewer.FileAssoc.Jpeg", "PhotoViewer.FileAssoc.Jpeg",
                                  "PhotoViewer.FileAssoc.Jpeg", "PhotoViewer.FileAssoc.Jpeg",
                                  "PhotoViewer.FileAssoc.Jpeg", "PhotoViewer.FileAssoc.Png",
                                  "PhotoViewer.FileAssoc.Tiff", "PhotoViewer.FileAssoc.Tiff",
                                  "PhotoViewer.FileAssoc.Wdp" };

            for (int i = 0; i < exts.Length; i++)
            {
                using var key = Registry.CurrentUser.CreateSubKey(
                    $@"Software\Classes\{exts[i]}\OpenWithProgids");
                if (enable)
                    key.SetValue(progids[i], new byte[0], RegistryValueKind.None);
                else
                    key.DeleteValue(progids[i], throwOnMissingValue: false);
            }
        }

        // ── 4. Ẩn icon taskbar không cần thiết ──────────────────────────────
        public static bool GetHideTaskbarIcons()
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer");
            return key?.GetValue("HideSCAMeetNow") is 1;
        }

        public static void SetHideTaskbarIcons(bool hide)
        {
            // Meet Now
            using (var key = Registry.CurrentUser.CreateSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer"))
                key.SetValue("HideSCAMeetNow", hide ? 1 : 0, RegistryValueKind.DWord);

            // Windows Ink Workspace button
            using (var key = Registry.CurrentUser.CreateSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\PenWorkspace"))
                key.SetValue("PenWorkspaceButtonDesiredVisibility", hide ? 0 : 1, RegistryValueKind.DWord);

            // Task View button
            using (var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", writable: true))
                key?.SetValue("ShowTaskViewButton", hide ? 0 : 1, RegistryValueKind.DWord);
        }

        // ── 5. Tắt tự động điều chỉnh độ sáng ──────────────────────────────
        public static bool GetAutoBrightness()
        {
            using var key = Registry.LocalMachine.OpenSubKey(
                @"SYSTEM\CurrentControlSet\Control\Power\Policy\Settings\Video\AdaptiveDisplayTimeout");
            var acVal = key?.GetValue("ACSettingIndex");
            return acVal is not 0;
        }

        public static async Task SetAutoBrightnessAsync(bool enable)
        {
            // powercfg sets adaptive brightness on all power schemes
            string state = enable ? "1" : "0";
            await Task.Run(() =>
            {
                RunCmd($"powercfg /setacvalueindex SCHEME_CURRENT SUB_VIDEO ADAPTBRIGHT {state}");
                RunCmd($"powercfg /setdcvalueindex SCHEME_CURRENT SUB_VIDEO ADAPTBRIGHT {state}");
                RunCmd("powercfg /setactive SCHEME_CURRENT");
            });
        }

        // ── 6. Xóa bàn phím ngôn ngữ khác (giữ US) ─────────────────────────
        public static async Task RemoveExtraKeyboardsAsync()
        {
            await RunPSAsync("Set-WinUserLanguageList -LanguageList en-US -Force");
        }

        // ── 7. Local Account Management ─────────────────────────────────────────

        public static string GetCurrentUserName() => Environment.UserName;

        public static bool GetIsCurrentUserAdmin()
        {
            var identity  = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        public static bool GetBuiltinAdminEnabled()
        {
            try
            {
                var psi = new ProcessStartInfo("powershell.exe",
                    "-NonInteractive -NoProfile -Command \"(Get-LocalUser -Name 'Administrator').Enabled\"")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };
                using var p = Process.Start(psi);
                var output = p?.StandardOutput.ReadToEnd().Trim() ?? "";
                p?.WaitForExit();
                return output.Equals("True", StringComparison.OrdinalIgnoreCase);
            }
            catch { return false; }
        }

        public static async Task RenameLocalAccountAsync(string oldName, string newName)
        {
            await RunPSAsync($"Rename-LocalUser -Name '{oldName}' -NewName '{newName}'");
        }

        public static async Task ChangeLocalPasswordAsync(string username, string newPassword)
        {
            // Encode password to base64 to avoid command-line special character issues
            var b64 = Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(newPassword));
            var script = $"$pwd = ConvertTo-SecureString ([System.Text.Encoding]::Unicode.GetString([System.Convert]::FromBase64String('{b64}'))) -AsPlainText -Force; Set-LocalUser -Name '{username}' -Password $pwd";
            await RunPSAsync(script);
        }

        public static async Task SetBuiltinAdminAsync(bool enable)
        {
            string state = enable ? "yes" : "no";
            await Task.Run(() => RunCmd($"net user Administrator /active:{state}"));
        }

        // ── helpers ──────────────────────────────────────────────────────────
        private static void RunCmd(string command)
        {
            var psi = new ProcessStartInfo("cmd.exe", $"/c {command}")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            using var p = Process.Start(psi);
            p?.WaitForExit();
        }

        private static Task RunPSAsync(string script) => Task.Run(() =>
        {
            var psi = new ProcessStartInfo("powershell.exe",
                $"-NonInteractive -NoProfile -Command \"{script}\"")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            using var p = Process.Start(psi);
            p?.WaitForExit();
        });
    }
}
