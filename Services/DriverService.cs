using System;
using System.Collections.Generic;
using System.Management;
using System.Threading.Tasks;

namespace TrickLor.Services
{
    public class DriverEntry
    {
        public string Name         { get; set; } = "";
        public string Version      { get; set; } = "";
        public string Manufacturer { get; set; } = "";
        public string Status       { get; set; } = "";
        public bool   IsSigned     { get; set; }
        public string SignedStr    => IsSigned ? "✅ Có" : "⚠️ Không";
    }

    public static class DriverService
    {
        public static async Task<List<DriverEntry>> GetDriversAsync() => await Task.Run(() =>
        {
            var list = new List<DriverEntry>();
            try
            {
                using var searcher = new ManagementObjectSearcher(
                    "SELECT DeviceName, DriverVersion, Manufacturer, Status, IsSigned " +
                    "FROM Win32_PnPSignedDriver WHERE DeviceName IS NOT NULL");
                foreach (ManagementObject obj in searcher.Get())
                {
                    var name = obj["DeviceName"]?.ToString() ?? "";
                    if (string.IsNullOrWhiteSpace(name)) continue;
                    list.Add(new DriverEntry
                    {
                        Name         = name,
                        Version      = obj["DriverVersion"]?.ToString() ?? "—",
                        Manufacturer = obj["Manufacturer"]?.ToString() ?? "—",
                        Status       = obj["Status"]?.ToString() ?? "—",
                        IsSigned     = obj["IsSigned"] is bool b && b,
                    });
                }
            }
            catch { }
            list.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            return list;
        });
    }
}
