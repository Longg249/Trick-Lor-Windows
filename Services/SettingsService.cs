using System.IO;
using System.Text.Json;

namespace WinDeployPro.Services
{
    public static class SettingsService
    {
        private static readonly string AppDataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "WinDeployPro");

        private static readonly string SettingsFile = Path.Combine(AppDataDir, "settings.json");

        private static Settings _current = new();

        static SettingsService()
        {
            Load();
        }

        public static bool IsDarkMode
        {
            get => _current.IsDarkMode;
            set
            {
                _current.IsDarkMode = value;
                Save();
            }
        }

        private static void Load()
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    var json = File.ReadAllText(SettingsFile);
                    _current = JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
                }
            }
            catch
            {
                // If loading fails, use default settings
                _current = new Settings();
            }
        }

        private static void Save()
        {
            try
            {
                Directory.CreateDirectory(AppDataDir);
                var json = JsonSerializer.Serialize(_current, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFile, json);
            }
            catch
            {
                // Silently fail if we can't save settings
            }
        }

        private class Settings
        {
            public bool IsDarkMode { get; set; } = true;
        }
    }
}
