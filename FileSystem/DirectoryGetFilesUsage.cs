// =============================================================================
// RULE ID   : cr-dotnet-0004
// RULE NAME : Directory.GetFiles Usage
// CATEGORY  : File System
// DESCRIPTION: Application uses Directory.GetFiles, Directory.EnumerateFiles or
//              similar methods to scan local directories. Assumes persistent
//              directory structures that may vary across cloud deployments.
// =============================================================================
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace SyntheticLegacyApp.FileSystem
{
    public class DirectoryGetFilesUsage
    {
        public IEnumerable<string> GetPendingReports()
        {
            // VIOLATION cr-dotnet-0004: Directory.GetFiles on local directory
            return Directory.GetFiles(@"C:\Reports\Pending", "*.pdf");
        }

        public IEnumerable<string> GetAllConfigFiles()
        {
            // VIOLATION cr-dotnet-0004: Directory.EnumerateFiles scanning local disk
            return Directory.EnumerateFiles(@"C:\Config\SyntheticApp", "*.xml",
                                            SearchOption.AllDirectories);
        }

        public IDictionary<string, long> GetFileSizes(string directory)
        {
            // VIOLATION cr-dotnet-0004: DirectoryInfo.GetFiles on arbitrary local path
            var dirInfo = new DirectoryInfo(directory);
            return dirInfo.GetFiles("*.*")
                          .ToDictionary(f => f.Name, f => f.Length);
        }

        public void MoveProcessedFiles(string sourceDir, string destDir)
        {
            // VIOLATION cr-dotnet-0004: Scanning and moving files - assumes local structure
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string dest = Path.Combine(destDir, Path.GetFileName(file));
                File.Move(file, dest);
            }
        }
    }
}
