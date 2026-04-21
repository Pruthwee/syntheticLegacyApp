// =============================================================================
// RULE ID   : cr-dotnet-0054
// RULE NAME : Hard-coded Temp Paths
// CATEGORY  : File System
// DESCRIPTION: FIXED - Replaced hard-coded paths with Path.GetTempPath() and Path.Combine()
// =============================================================================
using System;
using System.IO;

namespace SyntheticLegacyApp.FileSystem
{
    public class HardCodedTempPaths
    {
        private readonly string _tempDirectory;

        public HardCodedTempPaths()
        {
            // FIXED: Use cross-platform Path.GetTempPath() instead of hard-coded Windows paths
            _tempDirectory = Path.Combine(Path.GetTempPath(), "SyntheticApp");
            Directory.CreateDirectory(_tempDirectory);
        }

        public string CreateTempFile(string prefix)
        {
            // FIXED: Use Path.Combine for cross-platform path construction
            string tempFile = Path.Combine(
                _tempDirectory,
                $"{prefix}_{Guid.NewGuid():N}.tmp"
            );
            File.WriteAllText(tempFile, string.Empty);
            return tempFile;
        }

        public void WriteTempData(byte[] data, string filename)
        {
            // FIXED: Use Path.Combine instead of string concatenation with backslashes
            string path = Path.Combine(_tempDirectory, filename);
            File.WriteAllBytes(path, data);
        }

        public string GetTempExportPath()
        {
            // FIXED: Use Path.Combine for cross-platform compatibility
            string exportPath = Path.Combine(_tempDirectory, "exports");
            Directory.CreateDirectory(exportPath);
            return exportPath;
        }

        public void CleanupWindowsTemp()
        {
            // FIXED: Use Path.GetTempPath() instead of hard-coded Windows temp path
            try
            {
                foreach (string f in Directory.GetFiles(_tempDirectory, "synapp_*.tmp"))
                {
                    try
                    {
                        File.Delete(f);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to delete temp file {f}: {ex.Message}");
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                // Directory doesn't exist, nothing to clean up
            }
        }

        // Additional helper method for cloud-native temp file management
        public string CreateTempFileWithAutoCleanup(string prefix, out FileStream stream)
        {
            // FIXED: Create temp file with proper disposal pattern
            string tempFile = Path.Combine(
                _tempDirectory,
                $"{prefix}_{Guid.NewGuid():N}.tmp"
            );
            
            stream = new FileStream(
                tempFile,
                FileMode.Create,
                FileAccess.ReadWrite,
                FileShare.None,
                4096,
                FileOptions.DeleteOnClose // Auto-cleanup when stream is closed
            );
            
            return tempFile;
        }
    }
}
