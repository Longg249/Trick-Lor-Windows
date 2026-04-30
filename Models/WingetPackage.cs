namespace TrickLor.Models
{
    public class WingetPackage
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Version { get; set; } = "";
        public string Publisher { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsSelected { get; set; }
    }
}