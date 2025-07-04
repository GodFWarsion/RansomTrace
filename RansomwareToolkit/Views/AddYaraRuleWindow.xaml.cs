using System;
using System.IO;
using System.Windows;
using RansomwareToolkit.Helpers;

namespace RansomwareToolkit.Views
{
    public partial class AddYaraRuleWindow : Window
    {
        public AddYaraRuleWindow()
        {
            InitializeComponent();
        }

        private void SaveRule_Click(object sender, RoutedEventArgs e)
        {
            var rule = new YaraRule
            {
                Name = RuleNameBox.Text.Trim(),
                Author = AuthorBox.Text.Trim(),
                MalwareFamily = FamilyBox.Text.Trim(),
                Description = DescriptionBox.Text.Trim(),
                Strings = StringsBox.Text.Trim(),
                Condition = ConditionBox.Text.Trim()
            };

            if (string.IsNullOrWhiteSpace(rule.Name))
            {
                MessageBox.Show("Rule name is required.");
                return;
            }

            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Rules");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            string filePath = Path.Combine(folder, rule.Name + ".yar");
            File.WriteAllText(filePath, rule.ToYaraFormat());

            MessageBox.Show("YARA Rule saved successfully.");
            this.Close();
        }
    }
}
