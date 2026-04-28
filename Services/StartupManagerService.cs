using Microsoft.Win32;
using System.Collections.Generic;

namespace TrickLor.Services
{
    public class StartupEntry
    {
        public string Name    { get; set; } = "";
        public string Command { get; set; } = "";
        public string Source  { get; set; } = "";
    }

    public static class StartupManagerService
    {
        private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";

        public static List<StartupEntry> GetAll()
        {
            var list = new List<StartupEntry>();
            ReadFrom(Registry.CurrentUser,  RunKey, "HKCU (Người dùng)", list);
            ReadFrom(Registry.LocalMachine, RunKey, "HKLM (Hệ thống)",   list);
            return list;
        }

        private static void ReadFrom(RegistryKey hive, string path, string source, List<StartupEntry> list)
        {
            try
            {
                using var key = hive.OpenSubKey(path, writable: false);
                if (key == null) return;
                foreach (var name in key.GetValueNames())
                    list.Add(new StartupEntry
                    {
                        Name    = name,
                        Command = key.GetValue(name)?.ToString() ?? "",
                        Source  = source,
                    });
            }
            catch { }
        }

        public static void Remove(StartupEntry entry)
        {
            try
            {
                var hive = entry.Source.StartsWith("HKCU") ? Registry.CurrentUser : Registry.LocalMachine;
                using var key = hive.OpenSubKey(RunKey, writable: true);
                key?.DeleteValue(entry.Name, throwOnMissingValue: false);
            }
            catch { }
        }
    }
}
