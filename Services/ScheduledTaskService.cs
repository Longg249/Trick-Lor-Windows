using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace TrickLor.Services
{
    public class ScheduledTaskEntry
    {
        public string TaskName { get; set; } = "";
        public string NextRun  { get; set; } = "";
        public string Status   { get; set; } = "";
    }

    public static class ScheduledTaskService
    {
        public static async Task<List<ScheduledTaskEntry>> GetTasksAsync()
        {
            var list   = new List<ScheduledTaskEntry>();
            var output = await RunCmd("schtasks /query /fo CSV /nh");
            foreach (var line in output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                var cols = ParseCsv(line.Trim());
                if (cols.Length < 3 || string.IsNullOrWhiteSpace(cols[0])) continue;
                list.Add(new ScheduledTaskEntry { TaskName = cols[0], NextRun = cols[1], Status = cols[2] });
            }
            return list;
        }

        public static Task EnableAsync(string name)  => RunCmd($"schtasks /change /tn \"{name}\" /enable");
        public static Task DisableAsync(string name) => RunCmd($"schtasks /change /tn \"{name}\" /disable");
        public static Task DeleteAsync(string name)  => RunCmd($"schtasks /delete /tn \"{name}\" /f");

        private static string[] ParseCsv(string line)
        {
            var result = new List<string>();
            var sb     = new StringBuilder();
            bool inQ   = false;
            foreach (char c in line)
            {
                if      (c == '"')         { inQ = !inQ; }
                else if (c == ',' && !inQ) { result.Add(sb.ToString()); sb.Clear(); }
                else                       sb.Append(c);
            }
            result.Add(sb.ToString());
            return result.ToArray();
        }

        private static async Task<string> RunCmd(string cmd) => await Task.Run(() =>
        {
            var psi = new ProcessStartInfo("cmd.exe", $"/c {cmd}")
            {
                CreateNoWindow         = true,
                UseShellExecute        = false,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8,
            };
            using var proc = Process.Start(psi);
            var output = proc?.StandardOutput.ReadToEnd() ?? "";
            proc?.WaitForExit();
            return output;
        });
    }
}
