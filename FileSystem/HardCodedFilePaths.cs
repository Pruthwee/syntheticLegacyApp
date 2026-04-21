// =============================================================================
// RULE ID   : cr-dotnet-0001
// RULE NAME : Hard-coded File Paths
// CATEGORY  : File System
// DESCRIPTION: FIXED - Replaced hard-coded Windows file paths with environment
//              variables for cloud compatibility. Paths now configurable via
//              environment variables with fallback defaults.
// =============================================================================
using System;
using System.IO;

namespace SyntheticLegacyApp.FileSystem
{
    public class HardCodedFilePaths
    {
        // FIXED cr-dotnet-0001: Using environment variables with cross-platform defaults
        private static readonly string BaseDataDir  = Environment.GetEnvironmentVariable("APP_BASE_DATA_DIR")
            ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SyntheticApp", "Data");
        private static readonly string ReportOutDir = Environment.GetEnvironmentVariable("APP_REPORT_OUTPUT_DIR")
            ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Reports", "Output");
        private static readonly string ArchiveDir   = Environment.GetEnvironmentVariable("APP_ARCHIVE_DIR")
            ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Archives", "2024");

        public void ProcessInvoices()
        {
            // FIXED cr-dotnet-0001: Using environment variable for invoice path
            string invoicePath = Environment.GetEnvironmentVariable("APP_INVOICE_PATH")
                ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SyntheticApp", "Invoices", "pending");

            // Ensure directory exists
            if (!Directory.Exists(invoicePath))
            {
                Directory.CreateDirectory(invoicePath);
            }

            string[] files = Directory.GetFiles(invoicePath, "*.xml");
            foreach (string file in files)
            {
                string content = File.ReadAllText(file);
                // FIXED cr-dotnet-0001: Using environment variable for processed invoices path
                string processedDir = Environment.GetEnvironmentVariable("APP_PROCESSED_INVOICE_DIR")
                    ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProcessedInvoices");

                // Ensure directory exists
                if (!Directory.Exists(processedDir))
                {
                    Directory.CreateDirectory(processedDir);
                }

                string dest = Path.Combine(processedDir, Path.GetFileName(file));
                File.WriteAllText(dest, content);
            }
        }

        public string GetConfigFilePath(string configName)
        {
            // FIXED cr-dotnet-0001: Using environment variable with cross-platform path construction
            string configDir = Environment.GetEnvironmentVariable("APP_CONFIG_DIR")
                ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Config", "SyntheticApp");

            // Ensure directory exists
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }

            return Path.Combine(configDir, configName + ".xml");
        }
    }
}
