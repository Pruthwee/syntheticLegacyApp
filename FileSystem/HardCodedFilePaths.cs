// =============================================================================
// RULE ID   : cr-dotnet-0001
// RULE NAME : Hard-coded File Paths
// CATEGORY  : File System
// DESCRIPTION: FIXED - Replaced hard-coded paths with environment variables and Path.Combine
// =============================================================================
using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace SyntheticLegacyApp.FileSystem
{
    public class HardCodedFilePaths
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _baseDataDir;
        private readonly string _reportOutDir;
        private readonly string _archiveDir;
        private readonly string _s3Bucket;

        public HardCodedFilePaths(IAmazonS3 s3Client = null)
        {
            _s3Client = s3Client ?? new AmazonS3Client();
            
            // FIXED: Use environment variables with fallback to temp directory
            _baseDataDir = Environment.GetEnvironmentVariable("APP_DATA_DIR") ?? 
                          Path.Combine(Path.GetTempPath(), "SyntheticApp", "Data");
            _reportOutDir = Environment.GetEnvironmentVariable("REPORT_OUTPUT_DIR") ?? 
                           Path.Combine(Path.GetTempPath(), "Reports", "Output");
            _archiveDir = Environment.GetEnvironmentVariable("ARCHIVE_DIR") ?? 
                         Path.Combine(Path.GetTempPath(), "Archives", DateTime.UtcNow.Year.ToString());
            _s3Bucket = Environment.GetEnvironmentVariable("S3_BUCKET") ?? "app-data-bucket";
            
            // Ensure directories exist
            Directory.CreateDirectory(_baseDataDir);
            Directory.CreateDirectory(_reportOutDir);
            Directory.CreateDirectory(_archiveDir);
        }

        public async Task ProcessInvoicesAsync()
        {
            // FIXED: Use cross-platform path construction
            string invoicePath = Path.Combine(_baseDataDir, "Invoices", "pending");
            Directory.CreateDirectory(invoicePath);
            
            string[] files = Directory.GetFiles(invoicePath, "*.xml");
            foreach (string file in files)
            {
                string content = await File.ReadAllTextAsync(file);
                
                // FIXED: Use Path.Combine for cross-platform compatibility
                string destDir = Path.Combine(_reportOutDir, "ProcessedInvoices");
                Directory.CreateDirectory(destDir);
                string dest = Path.Combine(destDir, Path.GetFileName(file));
                await File.WriteAllTextAsync(dest, content);
                
                // Also upload to S3 for cloud-native storage
                await UploadToS3Async(file, $"invoices/processed/{Path.GetFileName(file)}");
            }
        }

        public void ProcessInvoices()
        {
            ProcessInvoicesAsync().GetAwaiter().GetResult();
        }

        public string GetConfigFilePath(string configName)
        {
            // FIXED: Use cross-platform Path.Combine
            var configDir = Environment.GetEnvironmentVariable("CONFIG_DIR") ?? 
                           Path.Combine(_baseDataDir, "Config");
            Directory.CreateDirectory(configDir);
            return Path.Combine(configDir, $"{configName}.xml");
        }

        private async Task UploadToS3Async(string filePath, string s3Key)
        {
            try
            {
                var request = new PutObjectRequest
                {
                    BucketName = _s3Bucket,
                    Key = s3Key,
                    FilePath = filePath
                };
                await _s3Client.PutObjectAsync(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to upload to S3: {ex.Message}");
                // Continue processing even if S3 upload fails
            }
        }
    }
}
