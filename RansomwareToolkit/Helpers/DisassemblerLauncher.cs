using System;
using System.Diagnostics;
using System.Windows;

public static class DisassemblerLauncher
{
    public static void Launch(string filePath)
    {
        // Show message box to choose disassembler
        MessageBoxResult result = MessageBox.Show("Choose Disassembler:\nYes = Ghidra\nNo = IDA Free", "Disassembler", MessageBoxButton.YesNoCancel);

        if (result == MessageBoxResult.Yes)
        {
            LaunchGhidra();  // FilePath is passed, but not used for now
        }
        else if (result == MessageBoxResult.No)
        {
            LaunchIDA(filePath);  // FilePath is passed, but not used for now
        }
        // Cancel = do nothing
    }

    // Method to launch Ghidra (filePath is passed but not used for now)
    private static void LaunchGhidra()
    {
        string ghidraBatPath = @"D:\PROGRAMMING\ghidra_11.1.2_PUBLIC_20240709\ghidra_11.1.2_PUBLIC\ghidraRun.bat";

        try
        {
            // Start the Ghidra batch file with UseShellExecute = false
            Process.Start(new ProcessStartInfo
            {
                FileName = ghidraBatPath,
                UseShellExecute = false,  // Set to false to allow batch file execution
                CreateNoWindow = true     // Don't show the command window
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show("Failed to open Ghidra: " + ex.Message, "Error");
        }
    }

    // Method to launch IDA Free (filePath is passed but not used for now)
    private static void LaunchIDA(string filePath)
    {
        string idaPath = @"C:\Program Files\IDA Free 9.0\ida.exe";

        try
        {
            // Start IDA
            Process.Start(new ProcessStartInfo
            {
                FileName = idaPath,
                UseShellExecute = true  // True works fine to launch an executable
            });

            MessageBox.Show("IDA Free GUI launched. Please load the file manually.", "Info");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Failed to open IDA: " + ex.Message, "Error");
        }
    }
}
