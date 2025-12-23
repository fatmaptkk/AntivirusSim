using System;
using System.IO;
using System.Linq;
using AntivirusSim.Models;
using AntivirusSim.Services;
using AntivirusSim.Storage;

namespace AntivirusSim
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Antivirus Simulation (NTP Project)");
            Console.WriteLine("Gerçek virüs içermez. İmza tabanlı tarama simülasyonudur.\n");

            var signatures = new[]
            {
                new VirusSignature("EICAR_SIM", "EICAR-TEST-FILE"),
                new VirusSignature("TROJAN_SIM", "trojan_signature"),
                new VirusSignature("WORM_SIM", "worm_signature")
            };

            string baseDir = AppContext.BaseDirectory;
            string projectRootGuess = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
            string sampleDir = Path.Combine(projectRootGuess, "SampleFiles");
            string quarantineDir = Path.Combine(projectRootGuess, "Quarantine");

            Directory.CreateDirectory(sampleDir);
            Directory.CreateDirectory(quarantineDir);

            CreateSampleFilesIfEmpty(sampleDir);

            Console.WriteLine("Taranacak klasör (Enter = varsayılan):");
            Console.WriteLine(sampleDir);
            Console.Write("\nYol: ");
            string? input = Console.ReadLine();
            string target = string.IsNullOrWhiteSpace(input) ? sampleDir : input.Trim();

            var engine = new SimpleAntivirusEngine(maxBytesToRead: 1024 * 1024);
            var quarantine = new QuarantineManager(quarantineDir);
            var controller = new ScanController(engine, quarantine);

            Console.WriteLine("\nTarama başlıyor...\n");
            var session = controller.ScanDirectory(target, signatures, quarantineInfected: true);

            int clean = 0, infected = 0, skipped = 0, error = 0;

            foreach (var r in session.Results)
            {
                switch (r.Status)
                {
                    case ScanStatus.Clean: clean++; break;
                    case ScanStatus.Infected: infected++; break;
                    case ScanStatus.Skipped: skipped++; break;
                    case ScanStatus.Error: error++; break;
                }

                Console.WriteLine($"[{r.Status}] {r.FileName}");

                if (r.Status == ScanStatus.Infected)
                    Console.WriteLine($"   Threat: {r.DetectedThreatName}");

                if (r.Quarantined)
                    Console.WriteLine($"   Quarantine: {r.QuarantinePath}");

                if (!string.IsNullOrWhiteSpace(r.ErrorMessage))
                    Console.WriteLine($"   Info: {r.ErrorMessage}");
            }

            Console.WriteLine("\n=== ÖZET ===");
            Console.WriteLine($"Clean     : {clean}");
            Console.WriteLine($"Infected  : {infected}");
            Console.WriteLine($"Skipped   : {skipped}");
            Console.WriteLine($"Error     : {error}");
            Console.WriteLine($"\nKarantina klasörü: {quarantineDir}");
            Console.WriteLine("\nÇıkmak için Enter...");
            Console.ReadLine();
        }

        private static void CreateSampleFilesIfEmpty(string sampleDir)
        {
            if (Directory.EnumerateFiles(sampleDir, "*", SearchOption.AllDirectories).Any())
                return;

            File.WriteAllText(Path.Combine(sampleDir, "clean_note.txt"),
                "Bu temiz bir dosyadır. Ders projesi için örnek içerik.");

            File.WriteAllText(Path.Combine(sampleDir, "infected_eicar.txt"),
                "Bu bir test dosyasıdır: EICAR-TEST-FILE");

            File.WriteAllText(Path.Combine(sampleDir, "maybe_trojan.txt"),
                "Şüpheli içerik: trojan_signature");
        }
    }
}
