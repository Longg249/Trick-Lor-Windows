using System;
using System.IO;

namespace TrickLor.Services
{
    public static class BackupRestoreService
    {
        public static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TrickLor", "settings.json");

        public static bool Export(string destPath)
        {
            try
            {
                if (!File.Exists(SettingsPath)) return false;
                File.Copy(SettingsPath, destPath, overwrite: true);
                return true;
            }
            catch { return false; }
        }

        public static bool Import(string srcPath)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
                File.Copy(srcPath, SettingsPath, overwrite: true);
                return true;
            }
            catch { return false; }
        }

        public static bool SettingsExist() => File.Exists(SettingsPath);
    }
}
