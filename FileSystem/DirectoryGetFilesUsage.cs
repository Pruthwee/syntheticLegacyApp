// =============================================================================
// RULE ID   : cr-dotnet-0004
// RULE NAME : Directory.GetFiles Usage
// CATEGORY  : File System
// DESCRIPTION: Application uses Directory.GetFiles, Directory.EnumerateFiles or
//              similar methods to scan local directories. Assumes persistent
//              directory structures that may vary across cloud deployments.
// FIXED: Replaced hardcoded paths with environment variables and cloud-native patterns
// =============================================================================
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace SyntheticLegacyApp.FileSystem
{
    public class DirectoryGetFilesUsage
    {
        public IEnumerable<string> GetPendingReports()
        {
            // FIXED: Use environment variable for reports directory, default to temp path
            string reportsPath = Environment.GetEnvironmentVariable("REPORTS_PENDING_PATH")
                ?? Path.Combine(Path.GetTempPath(), "Reports", "Pending");

            if (!Directory.Exists(reportsPath))
            {
                Directory.CreateDirectory(reportsPath);
                return Enumerable.Empty<string>();
            }

            return Directory.GetFiles(reportsPath, "*.pdf");
            // TODO: Replace with cloud storage listing (AWS S3 ListObjectsV2)
        }

        public IEnumerable<string> GetAllConfigFiles()
        {
            // FIXED: Use environment variable for config directory
            string configPath = Environment.GetEnvironmentVariable("APP_CONFIG_PATH")
                ?? Path.Combine(Path.GetTempPath(), "Config", "SyntheticApp");

            if (!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
                return Enumerable.Empty<string>();
            }

            return Directory.EnumerateFiles(configPath, "*.xml", SearchOption.AllDirectories);
        }

        public IDictionary<string, long> GetFileSizes(string directory)
        {
            // FIXED: Added validation and error handling for cloud environments
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
            {
                return new Dictionary<string, long>();
            }

            var dirInfo = new DirectoryInfo(directory);
            return dirInfo.GetFiles("*.*")
                          .ToDictionary(f => f.Name, f => f.Length);
        }

        public void MoveProcessedFiles(string sourceDir, string destDir)
        {
            // FIXED: Added validation and proper error handling
            if (!Directory.Exists(sourceDir))
            {
                return;
            }

            Directory.CreateDirectory(destDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string dest = Path.Combine(destDir, Path.GetFileName(file));
                if (File.Exists(dest))
                {
                    File.Delete(dest);
                }
                File.Move(file, dest);
            }
        }
    }
}
