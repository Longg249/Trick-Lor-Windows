using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace TrickLor.Services
{
    public record DiskInfo(string Drive, long TotalBytes, long FreeBytes)
    {
        public double UsedPercent => TotalBytes > 0 ? (double)(TotalBytes - FreeBytes) / TotalBytes * 100 : 0;
    }

    public record RamInfo(long TotalBytes, long UsedBytes)
    {
        public double UsedPercent => TotalBytes > 0 ? (double)UsedBytes / TotalBytes * 100 : 0;
    }

    public record BatteryInfo(int Percent, bool IsCharging, bool Present);

    public static class SystemInfoService
    {
        // ── P/Invoke ─────────────────────────────────────────────────────────
        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORYSTATUSEX
        {
            public uint  dwLength;
            public uint  dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [DllImport("kernel32.dll")] static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_POWER_STATUS
        {
            public byte ACLineStatus;
            public byte BatteryFlag;
            public byte BatteryLifePercent;
            public byte SystemStatusFlag;
            public uint BatteryLifeTime;
            public uint BatteryFullLifeTime;
        }

        [DllImport("kernel32.dll")] static extern bool GetSystemPowerStatus(out SYSTEM_POWER_STATUS lpSystemPowerStatus);

        // ── PerformanceCounter (lazy init) ───────────────────────────────────
        private static PerformanceCounter? _cpuCounter;

        // ── Machine info ─────────────────────────────────────────────────────
        public static string GetMachineName() => Environment.MachineName;

        public static string GetMachineId()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
                return key?.GetValue("MachineGuid")?.ToString() ?? "N/A";
            }
            catch { return "N/A"; }
        }

        public static string GetTimeZone() => TimeZoneInfo.Local.DisplayName;

        public static string GetOSVersion()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                string name    = key?.GetValue("ProductName")?.ToString()    ?? "Windows";
                string ver     = key?.GetValue("DisplayVersion")?.ToString() ?? "";
                string build   = key?.GetValue("CurrentBuildNumber")?.ToString() ?? "";
                return $"{name} {ver} (Build {build})".Trim();
            }
            catch { return Environment.OSVersion.VersionString; }
        }

        // ── RAM ──────────────────────────────────────────────────────────────
        public static RamInfo GetRam()
        {
            var ms = new MEMORYSTATUSEX { dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>() };
            GlobalMemoryStatusEx(ref ms);
            long total = (long)ms.ullTotalPhys;
            long avail = (long)ms.ullAvailPhys;
            return new RamInfo(total, total - avail);
        }

        // ── Disks ────────────────────────────────────────────────────────────
        public static List<DiskInfo> GetDisks()
        {
            var list = new List<DiskInfo>();
            foreach (var d in System.IO.DriveInfo.GetDrives())
                if (d.IsReady && d.DriveType == System.IO.DriveType.Fixed)
                    list.Add(new DiskInfo(d.Name, d.TotalSize, d.AvailableFreeSpace));
            return list;
        }

        // ── CPU ──────────────────────────────────────────────────────────────
        public static async Task<float> GetCpuAsync()
        {
            try
            {
                _cpuCounter ??= new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _cpuCounter.NextValue(); // first call is always 0
                await Task.Delay(500);
                return _cpuCounter.NextValue();
            }
            catch { return 0; }
        }

        // ── Battery ──────────────────────────────────────────────────────────
        public static BatteryInfo GetBattery()
        {
            GetSystemPowerStatus(out var s);
            bool present   = s.BatteryFlag != 128; // 128 = no battery
            bool charging  = s.ACLineStatus == 1;
            int  percent   = s.BatteryLifePercent == 255 ? 0 : s.BatteryLifePercent;
            return new BatteryInfo(percent, charging, present);
        }

        // ── CPU model name ───────────────────────────────────────────────────
        public static string GetCpuName()
        {
            try
            {
                using var s = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
                foreach (ManagementObject o in s.Get())
                    return o["Name"]?.ToString()?.Trim() ?? "N/A";
            }
            catch { }
            return "N/A";
        }

        // ── GPU model name ───────────────────────────────────────────────────
        public static string GetGpuName()
        {
            try
            {
                using var s = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
                foreach (ManagementObject o in s.Get())
                    return o["Name"]?.ToString()?.Trim() ?? "N/A";
            }
            catch { }
            return "N/A";
        }
    }
}
