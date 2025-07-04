using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace RansomwareToolkit.Views
{
    public partial class ReportView : UserControl
    {
        private string md5Hash;
        private string sha1Hash;
        private string sha256Hash;
        private string entropyText;
        private List<string> extractedStrings;
        private List<string> cryptoIndicators;
        private string verdictText;

        // Constructor that accepts the data from DashboardView
        public ReportView(string md5, string sha1, string sha256, string entropy, List<string> strings, List<string> indicators, string verdict)
        {
            InitializeComponent();

            md5Hash = md5;
            sha1Hash = sha1;
            sha256Hash = sha256;
            entropyText = entropy;
            extractedStrings = strings;
            cryptoIndicators = indicators;
            verdictText = verdict;

            // Set the report content into the TextBlock (or other UI elements)
            ReportText.Text = GenerateReportContent(); // This will generate and display the report content
        }
        private string GenerateReportContent()
        {
            var reportBuilder = new StringBuilder();

            // Section 1: Hashes
            reportBuilder.AppendLine("File Hashes")
                .AppendLine("------------------")
                .AppendLine($"MD5: {md5Hash}")
                .AppendLine($"SHA1: {sha1Hash}")
                .AppendLine($"SHA256: {sha256Hash}")
                .AppendLine();

            // Section 2: Extracted Strings
            reportBuilder.AppendLine("Extracted Strings")
                .AppendLine("------------------");
            foreach (var item in extractedStrings)
            {
                reportBuilder.AppendLine(item);
            }
            reportBuilder.AppendLine();

            // Section 3: Entropy and Crypto Indicators
            reportBuilder.AppendLine("Entropy and Crypto Indicators")
                .AppendLine("----------------------------")
                .AppendLine($"Entropy: {entropyText}")
                .AppendLine("Detected Crypto Indicators:");
            foreach (var item in cryptoIndicators)
            {
                reportBuilder.AppendLine(item);
            }
            reportBuilder.AppendLine();

            // Section 4: Crypto Verdict
            reportBuilder.AppendLine("Crypto Verdict")
                .AppendLine("----------------")
                .AppendLine(verdictText);

            return reportBuilder.ToString();
        }

        // Export the report to PDF
        private void ExportPDF_Click(object sender, RoutedEventArgs e)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Ransomware Report";

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            // Update the XFontStyle usage to use a valid value. Replace 'Regular' with 'XFontStyleEx.Regular' or another valid value.

            XFont titleFont = new XFont("Arial", 16, XFontStyleEx.Regular); // Corrected to use XFontStyleEx.Regular
                                                                        
            XFont normalFont = new XFont("Arial", 12, XFontStyleEx.Regular); // Added the style parameter for consistency

            double yPosition = 20;

            // Title
            gfx.DrawString("Ransomware Analysis Report", titleFont, XBrushes.Black, 20, yPosition);
            yPosition += 40;

            // Content
            gfx.DrawString("Findings:", normalFont, XBrushes.Black, 20, yPosition);
            yPosition += 20;

            // Add the report content (with headings and sections)
            var sections = GenerateReportContent().Split(new string[] { "\n\n" }, StringSplitOptions.None);
            foreach (var section in sections)
            {
                gfx.DrawString(section, normalFont, XBrushes.Black, 20, yPosition);
                yPosition += 20;
            }

            // Save the PDF to a file
            string pdfFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Report.pdf");
            document.Save(pdfFilePath);
            MessageBox.Show("Report exported as PDF to: " + pdfFilePath);
        }

        private void ExportHTML_Click(object sender, RoutedEventArgs e)
        {
            string htmlContent = @"
        <html>
        <head>
            <style>
                body { font-family: Arial, sans-serif; background-color: #f4f4f4; color: #333; }
                h1 { color: #5C6BC0; }
                h2 { color: #388E3C; }
                pre { background-color: #fff; padding: 10px; border: 1px solid #ddd; }
                .section { margin-top: 30px; }
            </style>
        </head>
        <body>
            <h1>Ransomware Analysis Report</h1>
            <h2>Findings:</h2>";

            htmlContent += "<div class='section'><h3>File Hashes</h3><pre>" + md5Hash + "\n" + sha1Hash + "\n" + sha256Hash + "</pre></div>";
            htmlContent += "<div class='section'><h3>Extracted Strings</h3><pre>";
            foreach (var item in extractedStrings)
            {
                htmlContent += item + "\n";
            }
            htmlContent += "</pre></div>";

            htmlContent += "<div class='section'><h3>Entropy and Crypto Indicators</h3><pre>" + entropyText + "\n";
            foreach (var item in cryptoIndicators)
            {
                htmlContent += item + "\n";
            }
            htmlContent += "</pre></div>";

            htmlContent += "<div class='section'><h3>Crypto Verdict</h3><pre>" + verdictText + "</pre></div>";

            htmlContent += "</body></html>";

            string htmlFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Report.html");
            File.WriteAllText(htmlFilePath, htmlContent);
            MessageBox.Show("Report exported as HTML to: " + htmlFilePath);
        }
    }
}
