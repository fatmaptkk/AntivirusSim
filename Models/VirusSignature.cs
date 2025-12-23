namespace AntivirusSim.Models
{
    public class VirusSignature
    {
        public string Name { get; }
        public string Pattern { get; }

        public VirusSignature(string name, string pattern)
        {
            Name = name;
            Pattern = pattern;
        }
    }
}
