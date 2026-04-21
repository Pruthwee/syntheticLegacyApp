// =============================================================================
// RULE ID   : cr-dotnet-0054
// RULE NAME : Hard-coded Temp Paths
// CATEGORY  : File System
// DESCRIPTION: Application contains hard-coded references to Windows temporary
//              directories like C:\Temp or assumes Windows-style path separators.
//              These paths do not exist on non-Windows cloud platforms.
// =============================================================================
using System.IO;

namespace SyntheticLegacyApp.FileSystem
{
    public class HardCodedTempPaths
    {
        // VIOLATION cr-dotnet-0054: Hard-coded Windows temp directories
        private const string TempDirectory     = @"C:\Temp\SyntheticApp";
        private const string WindowsTempFolder = @"C:\Windows\Temp";

        public string CreateTempFile(string prefix)
        {
            // VIOLATION cr-dotnet-0054: Hard-coded temp path with Windows backslash separator
            string tempFile = @"C:\Temp\" + prefix + "_" +
                              System.Guid.NewGuid().ToString("N") + ".tmp";
            File.WriteAllText(tempFile, string.Empty);
            return tempFile;
        }

        public void WriteTempData(byte[] data, string filename)
        {
            // VIOLATION cr-dotnet-0054: Windows path separator assumed
            string path = @"C:\Temp\SyntheticApp\" + filename;
            File.WriteAllBytes(path, data);
        }

        public string GetTempExportPath()
        {
            // VIOLATION cr-dotnet-0054: Returns hard-coded Windows temp path
            return @"C:\Temp\SyntheticApp\exports";
        }

        public void CleanupWindowsTemp()
        {
            // VIOLATION cr-dotnet-0054: Assuming Windows system temp path
            foreach (string f in Directory.GetFiles(WindowsTempFolder, "synapp_*.tmp"))
                File.Delete(f);
        }
    }
}
