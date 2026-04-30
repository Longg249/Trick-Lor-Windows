using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TrickLor.Services
{
    public static class OfficeInstallService
    {
        /// <summary>
        /// Chạy lệnh PowerShell "iex (irm setup.installoffice.org)" để cài Office.
        /// </summary>
        public static async Task RunAsync(IProgress<string> progress)
        {
            string command = "iex (irm setup.installoffice.org)";

            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
                UseShellExecute = true,
                Verb = "runas",                     // yêu cầu quyền Administrator
                WindowStyle = ProcessWindowStyle.Normal
            };

            try
            {
                progress.Report("Đang chạy script cài đặt Office từ installoffice.org...");
                var process = Process.Start(psi);
                await process.WaitForExitAsync();

                if (process.ExitCode == 0)
                    progress.Report("✅ Cài đặt Office hoàn tất.");
                else
                    progress.Report($"Script kết thúc với mã lỗi: {process.ExitCode}");
            }
            catch (Exception ex)
            {
                progress.Report($"Lỗi khi chạy script: {ex.Message}");
            }
        }
    }
}