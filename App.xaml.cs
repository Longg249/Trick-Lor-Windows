using System.Diagnostics;
using System.Security.Principal;
using System.Windows;
using TrickLor.Services;

namespace TrickLor
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Tự yêu cầu quyền Admin nếu chưa có
            if (!IsRunAsAdmin())
            {
                var psi = new ProcessStartInfo
                {
                    FileName        = Process.GetCurrentProcess().MainModule!.FileName,
                    UseShellExecute = true,
                    Verb            = "runas"   // hiện hộp thoại UAC
                };
                try
                {
                    Process.Start(psi);
                }
                catch
                {
                    // Người dùng bấm "No" trên UAC → chạy không có quyền admin
                }
                Shutdown();
                return;
            }

            ThemeService.Apply(SettingsService.IsDarkMode);
            ThemeService.ApplyAccent(SettingsService.AccentColor);
            base.OnStartup(e);
        }

        private static bool IsRunAsAdmin()
        {
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
