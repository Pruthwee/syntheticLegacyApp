// =============================================================================
// RULE ID   : cr-dotnet-0001
// RULE NAME : Hard-coded File Paths
// CATEGORY  : File System
// DESCRIPTION: Application contains absolute Windows file paths (C:\, D:\) or
//              hard-coded directory references that assume specific Windows file
//              system structures. In cloud environments these paths may not exist.
// =============================================================================
using System.IO;

namespace SyntheticLegacyApp.FileSystem
{
    public class HardCodedFilePaths
    {
        // VIOLATION cr-dotnet-0001: Absolute Windows drive-letter paths
        private static readonly string BaseDataDir  = @"C:\AppData\SyntheticApp\Data";
        private static readonly string ReportOutDir = @"D:\Reports\Output";
        private static readonly string ArchiveDir   = @"C:\Archives\2024";

        public void ProcessInvoices()
        {
            // VIOLATION cr-dotnet-0001: Hard-coded Windows path in method body
            string invoicePath = @"C:\AppData\SyntheticApp\Invoices\pending";
            string[] files = Directory.GetFiles(invoicePath, "*.xml");
            foreach (string file in files)
            {
                string content = File.ReadAllText(file);
                // VIOLATION cr-dotnet-0001: Target path also hard-coded
                string dest = @"D:\ProcessedInvoices\" + Path.GetFileName(file);
                File.WriteAllText(dest, content);
            }
        }

        public string GetConfigFilePath(string configName)
        {
            // VIOLATION cr-dotnet-0001: Windows path separator and drive letter
            return @"C:\Config\SyntheticApp\" + configName + ".xml";
        }
    }
}
