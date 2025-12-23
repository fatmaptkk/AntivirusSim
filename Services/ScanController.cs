using System;
using System.Collections.Generic;
using System.IO;
using AntivirusSim.Core;
using AntivirusSim.Models;
using AntivirusSim.Storage;

namespace AntivirusSim.Services
{
    public class ScanController
    {
        private readonly IAntivirusEngine _engine;
        private readonly QuarantineManager _quarantine;

        public ScanController(IAntivirusEngine engine, QuarantineManager quarantine)
        {
            _engine = engine;
            _quarantine = quarantine;
        }

        public ScanSession ScanDirectory(string directory, IEnumerable<VirusSignature> signatures, bool quarantineInfected = true)
        {
            var session = new ScanSession(directory);

            if (!Directory.Exists(directory))
            {
                session.AddResult(new ScanResult
                {
                    FilePath = directory,
                    FileName = "(directory)",
                    Status = ScanStatus.Error,
                    ErrorMessage = "Klasör bulunamadı."
                });
                return session;
            }

            foreach (var file in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
            {
                var item = new FileScanItem(file);
                var result = _engine.ScanFile(item, signatures);

                if (quarantineInfected && result.Status == ScanStatus.Infected)
                {
                    try
                    {
                        string qPath = _quarantine.QuarantineFile(file);
                        result.Quarantined = true;
                        result.QuarantinePath = qPath;
                    }
                    catch (Exception ex)
                    {
                        result.Quarantined = false;
                        result.ErrorMessage = "Karantina hatası: " + ex.Message;
                    }
                }

                session.AddResult(result);
            }

            return session;
        }
    }
}
