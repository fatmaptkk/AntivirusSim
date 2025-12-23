using System.Collections.Generic;
using AntivirusSim.Models;

namespace AntivirusSim.Core
{
    public class ScanSession
    {
        public string TargetDirectory { get; }
        public List<ScanResult> Results { get; } = new();

        public ScanSession(string targetDirectory)
        {
            TargetDirectory = targetDirectory;
        }

        public void AddResult(ScanResult result) => Results.Add(result);
    }
}
