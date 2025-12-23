namespace AntivirusSim.Models
{
    public class FileScanItem
    {
        public string FullPath { get; }
        public string FileName => System.IO.Path.GetFileName(FullPath);

        public FileScanItem(string fullPath)
        {
            FullPath = fullPath;
        }
    }
}
