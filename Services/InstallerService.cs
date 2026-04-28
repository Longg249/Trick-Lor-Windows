using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TrickLor.Services
{
    public static class InstallerService
    {
        public static async Task InstallListAsync(
            List<(string Name, string WingetId)> apps,
            IProgress<string>? progress = null)
        {
            foreach (var (name, id) in apps)
            {
                progress?.Report($"⏳ Đang cài: {name}");
                try
                {
                    await RunAsync($"winget install --id {id} -e --silent --accept-source-agreements --accept-package-agreements");
                    progress?.Report($"✅ Xong: {name}");
                }
                catch (Exception ex)
                {
                    progress?.Report($"❌ Lỗi {name}: {ex.Message}");
                }
            }
        }

        private static Task RunAsync(string command) => Task.Run(() =>
        {
            var psi = new ProcessStartInfo("cmd.exe", $"/c {command}")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
            };
            using var p = Process.Start(psi) ?? throw new Exception("Không thể chạy lệnh.");
            p.WaitForExit();
        });
    }
}
