using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using RansomwareToolkit.Helpers;


namespace RansomwareToolkit.Views
{
    public partial class DashboardView : UserControl
    {
        private string currentFilePath = "";
        private string sha256 = "";

        public DashboardView(string filePath)
        {
            InitializeComponent();
            currentFilePath = filePath;

            // Display basic data
            DisplayHashes(filePath);
            ExtractAndDisplayStrings(filePath);
            DetectEncryptionIndicators(filePath);

            // Integrate CryptoAnalyzer
            var result = CryptoAnalyzer.AnalyzeFile(filePath);
            CryptoAnalyzer.ExportReport(filePath, result);

            // Optional: Show verdict in UI (update UI directly instead of MessageBox)
            VerdictText.Text = $"Crypto Verdict: {result.Verdict}";
        }

        public void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            // Collect the required data from the UI
            string md5Hash = Md5HashText.Text;
            string sha1Hash = Sha1HashText.Text;
            string sha256Hash = Sha256HashText.Text;
            string entropyText = EntropyText.Text;
            string verdictText = VerdictText.Text;

            List<string> extractedStrings = new List<string>();
            foreach (var item in StringListBox.Items)
            {
                extractedStrings.Add(item.ToString());
            }

            List<string> cryptoIndicators = new List<string>();
            foreach (var item in CryptoIndicatorsListBox.Items)
            {
                cryptoIndicators.Add(item.ToString());
            }

            // Create and pass the data to ReportView
            ReportView reportView = new ReportView(md5Hash, sha1Hash, sha256Hash, entropyText, extractedStrings, cryptoIndicators, verdictText);
            this.Content = reportView; // Or use navigation in your framework
        }

        private void DisplayHashes(string filePath)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);

            using var md5 = MD5.Create();
            using var sha1 = SHA1.Create();
            using var sha256Alg = SHA256.Create();

            string md5Hash = BitConverter.ToString(md5.ComputeHash(fileBytes)).Replace("-", "");
            string sha1Hash = BitConverter.ToString(sha1.ComputeHash(fileBytes)).Replace("-", "");
            sha256 = BitConverter.ToString(sha256Alg.ComputeHash(fileBytes)).Replace("-", "");

            Md5HashText.Text = $"MD5: {md5Hash}";
            Sha1HashText.Text = $"SHA1: {sha1Hash}";
            Sha256HashText.Text = $"SHA256: {sha256}";
        }

        private void ExtractAndDisplayStrings(string filePath)
        {
            var strings = new List<string>();
            var sb = new StringBuilder();

            foreach (byte b in File.ReadAllBytes(filePath))
            {
                if (b >= 32 && b <= 126)
                {
                    sb.Append((char)b);
                }
                else
                {
                    if (sb.Length >= 3)
                        strings.Add(sb.ToString());
                    sb.Clear();
                }
            }

            if (sb.Length >= 3)
                strings.Add(sb.ToString());

            foreach (string s in strings)
            {
                StringListBox.Items.Add(s);
            }
        }

        private void OpenVirusTotal_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(sha256))
            {
                string vtUrl = $"https://www.virustotal.com/gui/file/{sha256}";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = vtUrl,
                    UseShellExecute = true
                });
            }
        }

        private void ExportStrings_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                FileName = "extracted_strings.txt",
                Filter = "Text File (*.txt)|*.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                using StreamWriter writer = new StreamWriter(dialog.FileName);
                foreach (var item in StringListBox.Items)
                {
                    writer.WriteLine(item);
                }

                MessageBox.Show("Strings exported successfully.");
            }
        }

        private double CalculateEntropy(byte[] data)
        {
            if (data == null || data.Length == 0)
                return 0.0;

            int[] counts = new int[256];
            foreach (byte b in data)
                counts[b]++;

            double entropy = 0.0;
            foreach (int count in counts)
            {
                if (count == 0) continue;
                double p = (double)count / data.Length;
                entropy -= p * Math.Log(p, 2);
            }
            return entropy;
        }

        private void DetectEncryptionIndicators(string filePath)
        {
            byte[] data = File.ReadAllBytes(filePath);
            double entropy = CalculateEntropy(data);
            EntropyText.Text = $"Entropy: {entropy:F3}";

            if (entropy > 7.5)
                CryptoIndicatorsListBox.Items.Add("High Entropy - Possible Packed/Encrypted File");

            // Search for known crypto-related strings
            var knownCryptoKeywords = new List<string> {
    // Algorithms
                "AES", "DES", "3DES", "RC4", "RC5", "RC6", "RSA", "ECC", "ECDSA", "ECDH", "Blowfish", "Twofish",
                "Serpent", "Salsa20", "ChaCha", "ChaCha20", "Camellia",
                "SHA1", "SHA256", "SHA512", "SHA3", "MD5", "Whirlpool",
                "HMAC", "CMAC", "GCM", "CBC", "CTR", "XTS", "CFB",

                // Key derivation
                "PBKDF2", "scrypt", "bcrypt", "Argon2",

                // Windows APIs
                "CryptEncrypt", "CryptDecrypt", "CryptImportKey", "CryptAcquireContext", "CryptGenRandom",
                "BCryptEncrypt", "BCryptDecrypt", "BCryptGenRandom", "BCryptGenerateSymmetricKey",
                "BCryptHashData", "HashCreate", "HashData",

                // OpenSSL / Libsodium / misc
                "EVP_Encrypt", "EVP_Decrypt", "AES_set_encrypt_key", "RAND_bytes", "mbedtls_cipher",
                "libsodium", "Crypto++", "libgcrypt", "OpenSSL", "Botan", "BouncyCastle", "mbedtls",

                // Generic keywords
                "encrypt", "decrypt", "encryption", "decryption", "cipher", "keygen", "key exchange",
                "public key", "private key", "symmetric", "asymmetric", "digital signature", "padding"
            };

            string fileContent = Encoding.ASCII.GetString(data);
            foreach (var keyword in knownCryptoKeywords)
            {
                if (fileContent.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    CryptoIndicatorsListBox.Items.Add($"Keyword Found: {keyword}");
                }
            }
        }

    }
}
