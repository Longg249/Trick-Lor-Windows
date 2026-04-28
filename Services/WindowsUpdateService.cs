using Microsoft.Win32;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TrickLor.Services
{
    public static class WindowsUpdateService
    {
        private const string AUKey = @"SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU";

        public static bool IsPaused()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(AUKey);
                return key?.GetValue("NoAutoUpdate") is 1;
            }
            catch { return false; }
        }

        public static void Pause()
        {
            using var key = Registry.LocalMachine.CreateSubKey(AUKey, writable: true);
            key.SetValue("NoAutoUpdate", 1, RegistryValueKind.DWord);
            key.SetValue("AUOptions",    1, RegistryValueKind.DWord);
        }

        public static void Resume()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(AUKey, writable: true);
                key?.DeleteValue("NoAutoUpdate", throwOnMissingValue: false);
                key?.DeleteValue("AUOptions",    throwOnMissingValue: false);
            }
            catch { }
        }

        public static Task OpenSettingsAsync() => Task.Run(() =>
            Process.Start(new ProcessStartInfo("ms-settings:windowsupdate") { UseShellExecute = true }));
    }
}
