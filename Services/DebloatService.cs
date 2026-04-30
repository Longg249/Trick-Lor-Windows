using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace TrickLor.Services
{
    public static class DebloatService
    {
        public static async Task<string> CreateRestorePointAsync(IProgress<string> progress)
        {
            return await Task.Run(() =>
            {
                try
                {
                    progress?.Report("Tạo điểm khôi phục hệ thống...");
                    var psi = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = "-Command \"Checkpoint-Computer -Description 'TrickLor Debloat' -RestorePointType MODIFY_SETTINGS\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    using var p = Process.Start(psi);
                    p.WaitForExit();
                    return p.ExitCode == 0 ? "✅ Đã tạo điểm khôi phục thành công" : "⚠️ Không thể tạo điểm khôi phục (có thể tính năng bị tắt)";
                }
                catch (Exception ex)
                {
                    return $"❌ Lỗi: {ex.Message}";
                }
            });
        }

        public static void OpenSystemRestoreUI()
        {
            Process.Start("rstrui.exe");
        }

        public static async Task RemoveAppxAsync(string packageName, IProgress<string> progress)
        {
            await Task.Run(() =>
            {
                progress?.Report($"Đang gỡ {packageName}...");
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"Get-AppxPackage *{packageName}* | Remove-AppxPackage\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                using var p = Process.Start(psi);
                p.WaitForExit();
                if (p.ExitCode != 0)
                    throw new Exception($"Lỗi khi gỡ {packageName}");
            });
        }

        public static async Task DisableChatWidgetAsync(IProgress<string> progress)
        {
            await Task.Run(() =>
            {
                progress?.Report("Đang tắt Teams Chat trên taskbar...");
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true);
                key?.SetValue("TaskbarMn", 0, RegistryValueKind.DWord);
            });
        }

        public static async Task RemoveOneDriveAsync(IProgress<string> progress)
        {
            await Task.Run(() =>
            {
                progress?.Report("Đang gỡ OneDrive...");
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-Command \"Stop-Process -Name OneDrive -Force -ErrorAction SilentlyContinue; Start-Sleep -Seconds 2; winget uninstall --id Microsoft.OneDrive --silent --accept-package-agreements; Remove-Item -Path \\\"$env:USERPROFILE\\OneDrive\\\" -Recurse -Force -ErrorAction SilentlyContinue\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process.Start(psi).WaitForExit();
            });
        }

        public static async Task DisableWidgetsAsync(IProgress<string> progress)
        {
            await Task.Run(() =>
            {
                progress?.Report("Đang tắt Widgets...");
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true);
                key?.SetValue("TaskbarDa", 0, RegistryValueKind.DWord);
            });
        }

        public static async Task DisableBingSearchAsync(IProgress<string> progress)
        {
            await Task.Run(() =>
            {
                progress?.Report("Đang tắt Bing Search trong Start Menu...");
                using var key = Registry.CurrentUser.CreateSubKey(@"Software\Policies\Microsoft\Windows\Explorer");
                key?.SetValue("DisableSearchBoxSuggestions", 1, RegistryValueKind.DWord);
            });
        }

        public static async Task<List<BloatwareItem>> GetInstalledBloatwareAsync()
        {
            var items = new List<BloatwareItem>();
            var packages = await GetAppxPackagesAsync();
            if (packages == null) return items;

            var bloatwareList = new (string Display, string Pattern)[]
            {
                ("Xbox App", "XboxApp"),
                ("Xbox Game Bar", "XboxGamingOverlay"),
                ("Xbox Game Overlay", "XboxGameOverlay"),
                ("Xbox Identity Provider", "XboxIdentityProvider"),
                ("Xbox TCUI", "Xbox.TCUI"),
                ("Solitaire Collection", "MicrosoftSolitaireCollection"),
                ("3D Viewer", "3DViewer"),
                ("Mixed Reality Portal", "MixedReality.Portal"),
                ("Groove Music", "ZuneMusic"),
                ("Movies & TV", "ZuneVideo"),
                ("Get Started / Tips", "Getstarted"),
                ("Skype", "SkypeApp"),
                ("People", "People"),
                ("Mail & Calendar", "windowscommunicationsapps"),
                ("Your Phone", "YourPhone"),
                ("Cortana", "549981C3F5F10"),
                ("Office Hub", "MicrosoftOfficeHub"),
                ("Feedback Hub", "WindowsFeedbackHub"),
                ("OneNote (Store)", "Office.OneNote"),
                ("Maps", "WindowsMaps"),
                ("Bing News", "BingNews"),
                ("Bing Weather", "BingWeather"),
                ("Candy Crush", "Candy"),
                ("Spotify", "SpotifyMusic"),
                ("Amazon", "Amazon.com.Amazon"),
                ("Facebook", "Facebook.Facebook"),
                ("TikTok", "TikTok"),
                ("LinkedIn", "LinkedIn")
            };

            foreach (var bloat in bloatwareList)
            {
                var pkg = packages.FirstOrDefault(p => p.Name?.Contains(bloat.Pattern, StringComparison.OrdinalIgnoreCase) == true);
                if (pkg != null)
                {
                    items.Add(new BloatwareItem
                    {
                        DisplayName = bloat.Display,
                        PackageName = pkg.PackageFullName,
                        IsSelected = false
                    });
                }
            }
            return items;
        }

        private static async Task<List<AppxPackage>> GetAppxPackagesAsync()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "-NoProfile -Command Get-AppxPackage | Select-Object Name, PackageFullName | ConvertTo-Json",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            using var p = Process.Start(psi);
            string json = await p.StandardOutput.ReadToEndAsync();
            await p.WaitForExitAsync();
            if (string.IsNullOrWhiteSpace(json)) return null;

            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<AppxPackage>>(json, options);
            }
            catch (JsonException)
            {
                var single = JsonSerializer.Deserialize<AppxPackage>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return single != null ? new List<AppxPackage> { single } : null;
            }
        }

        private class AppxPackage
        {
            public string Name { get; set; }
            public string PackageFullName { get; set; }
        }
    }

    public class BloatwareItem : System.ComponentModel.INotifyPropertyChanged
    {
        private bool _isSelected;
        public string DisplayName { get; set; }
        public string PackageName { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
    }
}