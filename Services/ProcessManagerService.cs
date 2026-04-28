using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TrickLor.Services
{
    public class ProcessEntry
    {
        public int    PID    { get; set; }
        public string Name   { get; set; } = "";
        public double RamMB  { get; set; }
        public string RamStr => $"{RamMB:F1} MB";
    }

    public static class ProcessManagerService
    {
        public static async Task<List<ProcessEntry>> GetProcessesAsync() => await Task.Run(() =>
        {
            var list = new List<ProcessEntry>();
            foreach (var p in Process.GetProcesses())
            {
                try
                {
                    list.Add(new ProcessEntry
                    {
                        PID   = p.Id,
                        Name  = p.ProcessName,
                        RamMB = p.WorkingSet64 / 1_048_576.0,
                    });
                }
                catch { }
                finally { p.Dispose(); }
            }
            list.Sort((a, b) => b.RamMB.CompareTo(a.RamMB));
            return list;
        });

        public static void Kill(int pid)
        {
            try { Process.GetProcessById(pid).Kill(); } catch { }
        }
    }
}
