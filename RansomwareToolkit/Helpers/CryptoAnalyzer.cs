using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PeNet;
using static System.Net.WebRequestMethods;

namespace RansomwareToolkit.Helpers
{
    public class CryptoAnalyzer
    {
        private static readonly List<string> CryptoApis = new List<string>
        {
            "BCryptEncrypt", "CryptEncrypt", "CryptAcquireContext",
            "CryptGenKey", "CryptDeriveKey", "CryptImportKey",
            "CryptExportKey", "BCryptGenerateSymmetricKey"
        };

        private static readonly List<string> CryptoStrings = new List<string>
        {
            "AES", "DES", "RSA", "RC4", "blowfish", "CryptAcquireContext",
            "BCryptEncrypt", "GenerateKey", "SHA256"
        };


        public class AnalysisResult
        {
            public List<string> ApiHits { get; set; } = new List<string>();

            public Dictionary<string, double> SectionEntropies { get; set; } = new Dictionary<string, double>();
            public List<string> StringHits { get; set; } = new List<string>();

            public string Verdict { get; set; }
        }

        public static AnalysisResult AnalyzeFile(string filePath)
        {
            var result = new AnalysisResult();
            var peFile = new PeFile(filePath);

            // Step 1: Import Table Crypto API Check
            result.ApiHits = peFile.ImportedFunctions?
                .Where(f => CryptoApis.Contains(f.Name))
                .Select(f => f.Name)
                .Distinct()
                .ToList() ?? new List<string>();

            // Step 2: Entropy Check for Key Sections
            byte[] rawBytes = System.IO.File.ReadAllBytes(filePath);
            // PeNet's Buff contains the full raw file as byte[]

            foreach (var section in peFile.ImageSectionHeaders)
            {
                var name = section.Name;
                if (name == ".text" || name == ".data" || name == ".rdata")
                {
                    int offset = (int)section.PointerToRawData;
                    int size = (int)section.SizeOfRawData;

                    if (offset + size <= rawBytes.Length)
                    {
                        var sectionData = rawBytes.Skip(offset).Take(size).ToArray();
                        double entropy = CalculateEntropy(sectionData);
                        result.SectionEntropies[name] = entropy;
                    }
                }
            }


            // Step 3: Simple Strings-based Detection
            var content = System.IO.File.ReadAllText(filePath);
            result.StringHits = CryptoStrings.Where(s => content.Contains(s)).ToList();

            // Step 4: Generate Verdict
            if (result.ApiHits.Any() || result.StringHits.Any() || result.SectionEntropies.Values.Any(e => e > 7.5))
            {
                result.Verdict = $"Detected - Possible crypto use ({string.Join(", ", result.ApiHits)})";
            }
            else
            {
                result.Verdict = "No obvious cryptographic indicators found.";
            }

            return result;
        }


        private static double CalculateEntropy(byte[] data)
        {
            int[] freq = new int[256];
            foreach (byte b in data) freq[b]++;

            double entropy = 0.0;
            int len = data.Length;
            foreach (int f in freq)
            {
                if (f == 0) continue;
                double p = (double)f / len;
                entropy -= p * Math.Log(p, 2);
            }
            return entropy;
        }

        public static void ExportReport(string filePath, AnalysisResult result, string outputDir = "reports")
        {
            Directory.CreateDirectory(outputDir);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string reportPath = Path.Combine(outputDir, $"{fileName}_crypto_report.txt");

            var sb = new StringBuilder();
            sb.AppendLine($"[+] File: {filePath}");
            sb.AppendLine($"[+] Crypto APIs Detected: {string.Join(", ", result.ApiHits)}");
            sb.AppendLine("[+] Suspicious Entropy:");
            foreach (var kvp in result.SectionEntropies)
                sb.AppendLine($"    - {kvp.Key}: {kvp.Value:F2}");

            sb.AppendLine($"[+] Strings Match: {string.Join(", ", result.StringHits)}");
            sb.AppendLine($"[+] Verdict: {result.Verdict}");

            System.IO.File.WriteAllText(reportPath, sb.ToString());
        }
    }
}
