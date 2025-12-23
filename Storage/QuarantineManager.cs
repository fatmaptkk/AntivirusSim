using System;
using System.IO;

namespace AntivirusSim.Storage
{
    public class QuarantineManager
    {
        public string QuarantineDirectory { get; }

        public QuarantineManager(string quarantineDirectory)
        {
            QuarantineDirectory = quarantineDirectory;
            Directory.CreateDirectory(QuarantineDirectory);
        }

        public string QuarantineFile(string filePath)
        {
            string name = Path.GetFileName(filePath);
            string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string destName = $"{stamp}__{name}";
            string destPath = Path.Combine(QuarantineDirectory, destName);

            File.Move(filePath, destPath, overwrite: true);
            return destPath;
        }
    }
}
