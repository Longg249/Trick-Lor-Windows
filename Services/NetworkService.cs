using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WinDeployPro.Services
{
    public static class NetworkService
    {
        public static Task ResetWinsockAsync()          => Run("netsh winsock reset");
        public static Task ResetTCPAsync()              => Run("netsh int ip reset");
        public static Task FlushDNSAsync()              => Run("ipconfig /flushdns");
        public static Task EnableNetworkDiscoveryAsync()=> Run(@"netsh advfirewall firewall set rule group=""Network Discovery"" new enable=Yes");
        public static Task EnableFileSharingAsync()     => Run(@"netsh advfirewall firewall set rule group=""File and Printer Sharing"" new enable=Yes");
        public static Task EnableSMBAsync()             => Run(@"PowerShell -Command ""Set-SmbServerConfiguration -EnableSMB2Protocol $true -Force""");
        public static Task RestartNetworkServicesAsync()=> Run("net stop server & net start server & net stop workstation & net start workstation");

        public static async Task FixAllAsync(IProgress<string>? p = null)
        {
            var steps = new (string Label, Func<Task> Action)[]
            {
                ("Reset Winsock",           ResetWinsockAsync),
                ("Reset TCP/IP",            ResetTCPAsync),
                ("Flush DNS",               FlushDNSAsync),
                ("Network Discovery",       EnableNetworkDiscoveryAsync),
                ("File & Printer Sharing",  EnableFileSharingAsync),
                ("Enable SMBv2",            EnableSMBAsync),
                ("Restart Services",        RestartNetworkServicesAsync),
            };

            foreach (var (label, action) in steps)
            {
                p?.Report($"⏳ {label}...");
                try
                {
                    await action();
                    p?.Report($"✅ {label} — Xong");
                }
                catch (Exception ex)
                {
                    p?.Report($"❌ {label} — Lỗi: {ex.Message}");
                }
            }
        }

        private static Task Run(string cmd) => Task.Run(() =>
        {
            var psi = new ProcessStartInfo("cmd.exe", $"/c {cmd}")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
            };
            using var p = Process.Start(psi) ?? throw new Exception("Không thể chạy lệnh.");
            p.WaitForExit();
        });
    }
}
