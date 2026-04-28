using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TrickLor.Services
{
    public class EventEntry
    {
        public string Time    { get; set; } = "";
        public string Level   { get; set; } = "";
        public string Source  { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public static class EventLogViewerService
    {
        public static async Task<List<EventEntry>> GetEntriesAsync(string logName, bool errorsOnly, int maxCount = 300)
            => await Task.Run(() =>
            {
                var list = new List<EventEntry>();
                try
                {
                    using var log = new EventLog(logName);
                    int total = log.Entries.Count;
                    for (int i = total - 1; i >= 0 && list.Count < maxCount; i--)
                    {
                        var e = log.Entries[i];
                        if (errorsOnly && e.EntryType == EventLogEntryType.Information) continue;
                        list.Add(new EventEntry
                        {
                            Time    = e.TimeGenerated.ToString("dd/MM HH:mm:ss"),
                            Level   = e.EntryType.ToString(),
                            Source  = e.Source,
                            Message = e.Message.Split('\n')[0].Trim(),
                        });
                    }
                }
                catch { }
                return list;
            });
    }
}
