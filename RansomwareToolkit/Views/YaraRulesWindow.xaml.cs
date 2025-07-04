using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using RansomwareToolkit.Helpers;
using RansomwareToolkit.Views;


namespace RansomwareToolkit
{
    public partial class YaraRulesWindow : Window
    {
        public YaraRulesWindow()
        {
            InitializeComponent();
            LoadYaraRules();
        }

        private void LoadYaraRules()
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Rules");
            if (!Directory.Exists(folder)) return;

            var rules = new List<YaraRule>();

            foreach (var file in Directory.GetFiles(folder, "*.yar"))
            {
                var rule = new YaraRule
                {
                    FilePath = file,
                    Name = Path.GetFileNameWithoutExtension(file)
                };

                foreach (var line in File.ReadAllLines(file))
                {
                    if (line.Contains("description")) rule.Description = ExtractValue(line);
                    if (line.Contains("author")) rule.Author = ExtractValue(line);
                    if (line.Contains("malware_family")) rule.MalwareFamily = ExtractValue(line);
                }

                rules.Add(rule);
            }

            YaraRulesDataGrid.ItemsSource = rules;
        }

        private string ExtractValue(string line)
        {
            return line.Split('=').Last().Trim().Trim('"');
        }

        private void AddYaraRuleButton_Click(object sender, RoutedEventArgs e)
        {
            new AddYaraRuleWindow().ShowDialog();
            LoadYaraRules(); // Refresh
        }

        private void RemoveYaraRuleButton_Click(object sender, RoutedEventArgs e)
        {
            if (YaraRulesDataGrid.SelectedItem is YaraRule selectedRule)
            {
                File.Delete(selectedRule.FilePath);
                LoadYaraRules();
                MessageBox.Show("Rule removed.");
            }
        }

        private void UseSelectedRuleButton_Click(object sender, RoutedEventArgs e)
        {
            if (YaraRulesDataGrid.SelectedItem is YaraRule selectedRule)
            {
                MessageBox.Show($"You selected rule: {selectedRule.Name}");
                // You can use this to pass to a scanner or pipeline
            }
        }
    }
}
