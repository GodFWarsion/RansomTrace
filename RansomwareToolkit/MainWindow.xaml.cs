using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;
using RansomwareToolkit.Views;
using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using RansomwareToolkit.Helpers;


namespace RansomwareToolkit
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            lastImportedPath = string.Empty;
            InitializeWebView2();
        }

        // MouseDown event handler for the draggable title bar
        private void TitleBar_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Start dragging the window when the mouse is clicked
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private string lastImportedPath; // Add this field at class level


        private void ImportPE_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "PE Files (*.exe;*.dll)|*.exe;*.dll|All files (*.*)|*.*";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                lastImportedPath = dlg.FileName; // This assigns the path when a file is selected
                MessageBox.Show("Imported File: " + lastImportedPath);

                ButtonPanel.Visibility = Visibility.Visible;

                // Load DashboardView
                DashboardView dashboard = new DashboardView(lastImportedPath);
                MainContentArea.Children.Clear();
                MainContentArea.Children.Add(dashboard);

                //DisassemblerLauncher.Launch(lastImportedPath);
            }
        }
        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(lastImportedPath))
            {
                var dashboard = new DashboardView(lastImportedPath);
                ReplaceMainContent(dashboard);
            }
            else
            {
                MessageBox.Show("Please import a file first.");
            }
        }

        private void RunAnalysisButton_Click(object sender, RoutedEventArgs e)
        {
            // Ensure a file is imported before running analysis
            if (string.IsNullOrEmpty(lastImportedPath))
            {
                MessageBox.Show("Please import a file before running the analysis.");
            }
            else
            {
                // Trigger the analysis function
                MessageBox.Show("Analysis started.");
                // Call your analysis function here
                // StartAnalysis(lastImportedPath);
            }
        }
        private void Yara_Click(object sender, RoutedEventArgs e)
        {
            // Create and show the YARA Rules window when the button is clicked
            YaraRulesWindow yaraWindow = new YaraRulesWindow();
            yaraWindow.Show(); // Show the YARA Rules window
        }


        private void ReplaceMainContent(UserControl view)
        {
            if (this.Content is Border border && border.Child is DockPanel mainDock)
            {
                var contentPanel = mainDock.Children.OfType<DockPanel>().FirstOrDefault();
                if (contentPanel != null)
                {
                    var grid = contentPanel.Children.OfType<Grid>().FirstOrDefault();
                    if (grid != null)
                    {
                        grid.Children.Clear();
                        grid.Children.Add(view);
                    }
                }
            }
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("RansomwareToolkit v1.0\nCreated for malware analysis education.", "About");
        }

        private void MITRE_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://attack.mitre.org/",
                UseShellExecute = true
            });
        }
        private void RunAnalysis_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Running analysis...");
        }

        private void SearchYara_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var yaraRulesWindow = new YaraRulesWindow();
                if (!yaraRulesWindow.IsVisible)
                {
                    yaraRulesWindow.Owner = this;
                    yaraRulesWindow.Show();
                }
                else
                {
                    yaraRulesWindow.Activate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                // Optionally, log the exception details
            }
        }

        private async void InitializeWebView2()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string webAssetsFolder = Path.Combine(appDirectory, "WebAssets");
            string indexHtmlPath = Path.Combine(webAssetsFolder, "index.html");

            if (File.Exists(indexHtmlPath))
            {
                // Ensure WebView2 is initialized first
                await PlaybookWebView.EnsureCoreWebView2Async(null);

                // Now set the source to the local HTML file
                PlaybookWebView.Source = new Uri("http://localhost:8000/index.html");

                // Optional: Open developer tools for debugging purposes
                //PlaybookWebView.CoreWebView2.OpenDevToolsWindow();
            }
            else
            {
                MessageBox.Show("The playbook HTML file could not be found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Playbook_Click(object sender, RoutedEventArgs e)
        {
            // Show the WebView2 for Ransomware Playbook
            PlaybookWebView.Visibility = Visibility.Visible;

            // Optionally hide the side panel or any previous content
            ButtonPanel.Visibility = Visibility.Collapsed;
            MainContentArea.Children.Clear();  // Clear previous content

            // Add the WebView2 control to the MainContentArea (if not already added)
            MainContentArea.Children.Add(PlaybookWebView);
        }


        private void Plugins_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Plugins feature under development.", "Info");
        }
        private void Disassembler_Click(object sender, RoutedEventArgs e)
        {
            DisassemblerLauncher.Launch(lastImportedPath);
        }
        private void MobileTools_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Mobile Malware Tools launching soon.", "Info");
        }

       private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Ensure you can access SearchBox here
            var query = SearchBox.Text; // Ensure SearchBox is defined in XAML
            MessageBox.Show($"Searching for: {query}");
        }
        private string GenerateReportContent()
        {
            // Placeholder content for the report
            // In a real-world scenario, this content will be generated based on the imported file and analysis
            return "Sample Report\n\nHere is some analysis data...\n\nDetails of the report...";
        }



        private void Report_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder data to match the constructor parameters
            string md5 = "";
            string sha1 = "";
            string sha256 = "";
            string entropy = "";
            List<string> extractedStrings = new List<string>();  // Placeholder, replace with actual data
            List<string> cryptoIndicators = new List<string>();  // Placeholder, replace with actual data
            string verdict = "";  // Placeholder, replace with actual data

            // Generate the content for the report (can be dynamic based on analysis results)
            string reportContent = GenerateReportContent();  // Replace with real report generation logic

            // Create a ReportView instance and pass the necessary data
            ReportView reportView = new ReportView(md5, sha1, sha256, entropy, extractedStrings, cryptoIndicators, verdict);

            // Replace the MainContentArea's content with the report view
            ReplaceMainContent(reportView);
        }



        // Event handler for Sigma Rules menu item
        private void Sigma_Click(object sender, RoutedEventArgs e)
        {
            // Check if SigmaRulesWindow is already open
            var sigmaWindow = Application.Current.Windows.OfType<SigmaRulesWindow>().FirstOrDefault();

            if (sigmaWindow == null)
            {
                sigmaWindow = new SigmaRulesWindow();
                sigmaWindow.Owner = this;  // Set the MainWindow as the owner
                sigmaWindow.Show();
            }
            else
            {
                sigmaWindow.Activate();  // Bring the window to the front if already open
            }
        }


        // Event handler for Debugger menu item
        private void Debugger_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Debugger clicked.");
        }

        // Event handler for Disassembler menu item
        

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Maximize button click event handler
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            // Toggle between maximized and normal window states
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }
    }
}

