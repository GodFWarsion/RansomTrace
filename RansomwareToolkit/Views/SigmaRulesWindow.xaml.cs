using System.Windows;
using System.Windows.Input;

namespace RansomwareToolkit
{
    public partial class SigmaRulesWindow : Window
    {
        public SigmaRulesWindow()
        {
            InitializeComponent();
        }

        // Window MouseDown event to allow dragging the window
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove(); // Allows dragging the window when left mouse button is pressed
            }
        }

        // Title Bar MouseDown event to allow dragging
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove(); // Allows dragging the window when title bar is clicked
            }
        }

        // Minimize Button Click event
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized; // Minimizes the window
        }

        // Maximize Button Click event
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized; // Maximizes the window
            }
            else
            {
                this.WindowState = WindowState.Normal; // Restores the window to normal size
            }
        }

        // Close Button Click event
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Closes the SigmaRulesWindow
        }

        // Add Sigma Rule Button Click event
        private void AddSigmaRuleButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement logic to add a new Sigma rule
        }

        // Remove Sigma Rule Button Click event
        private void RemoveSigmaRuleButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement logic to remove a selected Sigma rule
        }

        // Use Selected Sigma Rule Button Click event
        private void UseSelectedSigmaRuleButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement logic to use the selected Sigma rule
        }
    }
}
