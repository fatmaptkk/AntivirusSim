using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AntivirusSim.Models;

namespace AntivirusSim.Services
{
    public class SimpleAntivirusEngine : IAntivirusEngine
    {
        private readonly int _maxBytesToRead;

        public SimpleAntivirusEngine(int maxBytesToRead = 1024 * 1024)
        {
            _maxBytesToRead = maxBytesToRead;
        }

        public ScanResult ScanFile(FileScanItem item, IEnumerable<VirusSignature> signatures)
        {
            var result = new ScanResult
            {
                FilePath = item.FullPath,
                FileName = item.FileName
            };

            try
            {
                string content = ReadFileSafe(item.FullPath, _maxBytesToRead).ToLowerInvariant();

                var hit = signatures.FirstOrDefault(s =>
                    content.Contains(s.Pattern.ToLowerInvariant()));

                if (hit != null)
                {
                    result.Status = ScanStatus.Infected;
                    result.DetectedThreatName = hit.Name;
                }
                else
                {
                    result.Status = ScanStatus.Clean;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                result.Status = ScanStatus.Skipped;
                result.ErrorMessage = "Erişim reddedildi: " + ex.Message;
            }
            catch (Exception ex)
            {
                result.Status = ScanStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private static string ReadFileSafe(string path, int maxBytes)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            int toRead = (int)Math.Min(fs.Length, maxBytes);

            byte[] buffer = new byte[toRead];
            int read = fs.Read(buffer, 0, toRead);

            return Encoding.UTF8.GetString(buffer, 0, read);
        }
    }
}
