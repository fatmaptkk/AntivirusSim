namespace AntivirusSim.Models
{
    public class ScanResult
    {
        public string FilePath { get; set; } = "";
        public string FileName { get; set; } = "";
        public ScanStatus Status { get; set; }
        public string? DetectedThreatName { get; set; }
        public string? ErrorMessage { get; set; }
        public bool Quarantined { get; set; }
        public string? QuarantinePath { get; set; }
    }
}
