// =============================================================================
// RULE ID   : cr-dotnet-0054
// RULE NAME : Hard-coded Temp Paths
// CATEGORY  : File System
// DESCRIPTION: Application contains hard-coded references to Windows temporary
//              directories like C:\Temp or assumes Windows-style path separators.
//              These paths do not exist on non-Windows cloud platforms.
// FIXED: Replaced hardcoded Windows paths with cross-platform Path.GetTempPath()
// =============================================================================
using System;
using System.IO;

namespace SyntheticLegacyApp.FileSystem
{
    public class HardCodedTempPaths
    {
        // FIXED: Use cross-platform temp directory
        private readonly string TempDirectory = Path.Combine(Path.GetTempPath(), "SyntheticApp");
        private readonly string AppTempFolder = Path.Combine(Path.GetTempPath(), "SyntheticApp");

        public HardCodedTempPaths()
        {
            Directory.CreateDirectory(TempDirectory);
        }

        public string CreateTempFile(string prefix)
        {
            // FIXED: Use Path.GetTempPath() for cross-platform compatibility
            string tempFile = Path.Combine(TempDirectory, $"{prefix}_{Guid.NewGuid():N}.tmp");
            File.WriteAllText(tempFile, string.Empty);
            return tempFile;
        }

        public void WriteTempData(byte[] data, string filename)
        {
            // FIXED: Use Path.Combine for cross-platform path construction
            string tempDir = Path.Combine(Path.GetTempPath(), "SyntheticApp");
            Directory.CreateDirectory(tempDir);
            string path = Path.Combine(tempDir, filename);
            File.WriteAllBytes(path, data);
        }

        public string GetTempExportPath()
        {
            // FIXED: Use cross-platform temp path
            string exportPath = Path.Combine(Path.GetTempPath(), "SyntheticApp", "exports");
            Directory.CreateDirectory(exportPath);
            return exportPath;
        }

        public void CleanupWindowsTemp()
        {
            // FIXED: Use application-specific temp directory
            string appTempPath = Path.Combine(Path.GetTempPath(), "SyntheticApp");
            if (!Directory.Exists(appTempPath)) return;

            try
            {
                foreach (string f in Directory.GetFiles(appTempPath, "synapp_*.tmp"))
                    File.Delete(f);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error cleaning temp files: {ex.Message}");
            }
        }
    }
}
