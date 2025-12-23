using System.Collections.Generic;
using AntivirusSim.Models;

namespace AntivirusSim.Services
{
    public interface IAntivirusEngine
    {
        ScanResult ScanFile(FileScanItem item, IEnumerable<VirusSignature> signatures);
    }
}
