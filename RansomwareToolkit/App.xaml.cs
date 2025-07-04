using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RansomwareToolkit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Unhandled exception: {e.Exception.Message}");
            // Optionally log the exception to a file or database
            e.Handled = true;  // Prevent the application from crashing
        }
    }
}
