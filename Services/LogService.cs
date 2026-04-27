using System;
using System.Collections.Generic;

namespace WinDeployPro.Services
{
    public class LogEntry
    {
        public string Time    { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public static class LogService
    {
        public static List<LogEntry> Entries { get; } = new();

        public static void Add(string message)
        {
            Entries.Add(new LogEntry
            {
                Time    = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                Message = message
            });
        }

        public static void Clear() => Entries.Clear();
    }
}
