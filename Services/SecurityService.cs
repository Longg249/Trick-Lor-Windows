using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WinDeployPro.Services
{
    public static class SecurityService
    {
        // ── Toast notifications ──────────────────────────────────────────────
        public static bool GetToast()
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\PushNotifications");
            return key?.GetValue("ToastEnabled") is not 0;
        }

        public static void SetToast(bool enable)
        {
            using var key = Registry.CurrentUser.CreateSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\PushNotifications");
            key.SetValue("ToastEnabled", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        // ── UAC ─────────────────────────────────────────────────────────────
        public static bool GetUAC()
        {
            using var key = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System");
            return key?.GetValue("EnableLUA") is not 0;
        }

        public static void SetUAC(bool enable)
        {
            using var key = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", writable: true);
            key?.SetValue("EnableLUA", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        // ── SmartScreen ──────────────────────────────────────────────────────
        public static bool GetSmartScreen()
        {
            using var key = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer");
            var val = key?.GetValue("SmartScreenEnabled")?.ToString();
            return !string.Equals(val, "Off", StringComparison.OrdinalIgnoreCase);
        }

        public static void SetSmartScreen(bool enable)
        {
            using var key = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer", writable: true);
            key?.SetValue("SmartScreenEnabled", enable ? "On" : "Off", RegistryValueKind.String);
        }

        // ── Security Alerts (Action Center) ─────────────────────────────────
        public static bool GetSecurityAlerts()
        {
            using var key = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer");
            return key?.GetValue("HideSCAHealth") is not 1;
        }

        public static void SetSecurityAlerts(bool enable)
        {
            using var key = Registry.LocalMachine.CreateSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer");
            key.SetValue("HideSCAHealth", enable ? 0 : 1, RegistryValueKind.DWord);
        }

        // ── Firewall ─────────────────────────────────────────────────────────
        public static bool GetFirewall()
        {
            try
            {
                var psi = new ProcessStartInfo("netsh", "advfirewall show allprofiles state")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };
                using var p = Process.Start(psi)!;
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                return output.Contains("ON", StringComparison.OrdinalIgnoreCase);
            }
            catch { return true; }
        }

        public static async Task SetFirewallAsync(bool enable)
        {
            string state = enable ? "on" : "off";
            await RunAsync($"netsh advfirewall set allprofiles state {state}");
        }

        private static Task RunAsync(string command) => Task.Run(() =>
        {
            var psi = new ProcessStartInfo("cmd.exe", $"/c {command}")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                Verb = "runas"
            };
            using var p = Process.Start(psi) ?? throw new Exception("Không thể chạy lệnh.");
            p.WaitForExit();
        });
    }
}
