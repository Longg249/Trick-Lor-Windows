using System;
using System.Diagnostics;
using System.Security.Principal;
using Microsoft.UI.Xaml;
using TrickLor.Services;

namespace TrickLor
{
    public partial class App : Application
    {
        public static MainWindow? MainWindow { get; private set; }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Bắt crash và ghi vào C:\crash.txt
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                System.IO.File.WriteAllText(@"C:\crash.txt", ex?.ToString() ?? "unknown error");
            };
            Microsoft.UI.Xaml.Application.Current.UnhandledException += (s, e) =>
            {
                e.Handled = true;
                System.IO.File.WriteAllText(@"C:\crash.txt", e.Exception?.ToString() ?? "unknown");
            };

            if (!IsRunAsAdmin())
            {
                var psi = new ProcessStartInfo
                {
                    FileName        = Process.GetCurrentProcess().MainModule!.FileName,
                    UseShellExecute = true,
                    Verb            = "runas"
                };
                try { Process.Start(psi); } catch { }
                Current.Exit();
                return;
            }

            ThemeService.Apply(SettingsService.IsDarkMode);
            ThemeService.ApplyAccent(SettingsService.AccentColor);

            MainWindow = new MainWindow();
            MainWindow.Activate();
        }

        private static bool IsRunAsAdmin()
        {
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
