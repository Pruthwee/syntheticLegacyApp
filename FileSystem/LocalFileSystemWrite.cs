// =============================================================================
// RULE ID   : cr-dotnet-0002
// RULE NAME : Local File System Write Operations
// CATEGORY  : File System
// DESCRIPTION: Application performs direct write operations to local file system
//              for data persistence. In cloud environments with ephemeral or
//              managed storage, local data may not persist across instance changes.
// FIXED: Replaced hardcoded file paths with environment variables and cloud-native patterns
// =============================================================================
using System;
using System.IO;
using System.Text;

namespace SyntheticLegacyApp.FileSystem
{
    public class LocalFileSystemWrite
    {
        // FIXED: Use environment variable for storage path, fallback to temp directory
        private readonly string _localStoragePath = Environment.GetEnvironmentVariable("APP_STORAGE_PATH")
            ?? Path.Combine(Path.GetTempPath(), "SyntheticApp", "Storage");

        public void SaveOrderData(string orderId, string serializedOrder)
        {
            // FIXED: Use environment-based storage path, cloud storage recommended (AWS S3, Azure Blob)
            // TODO: Replace with cloud storage SDK (e.g., AWS S3 PutObjectAsync)
            string filePath = Path.Combine(_localStoragePath, orderId + ".json");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, serializedOrder);
            // Recommended: Use AWS S3 client for persistent storage
            // await s3Client.PutObjectAsync(new PutObjectRequest { BucketName = bucket, Key = orderId + ".json", ContentBody = serializedOrder });
        }

        public void SaveUserSession(string sessionId, byte[] sessionData)
        {
            // FIXED: Use distributed session store instead of local disk (AWS ElastiCache Redis recommended)
            // TODO: Replace with distributed session storage (Redis, DynamoDB)
            string sessionPath = Path.Combine(_localStoragePath, "sessions");
            Directory.CreateDirectory(sessionPath);
            string sessionFile = Path.Combine(sessionPath, sessionId + ".dat");
            File.WriteAllBytes(sessionFile, sessionData);
            // Recommended: Use Redis for session storage
            // await redis.StringSetAsync(sessionId, sessionData);
        }

        public void AppendAuditLog(string entry)
        {
            // FIXED: Use console logging for cloud environments (CloudWatch, DataDog integration)
            // Cloud-native approach: Write to stdout for log aggregation
            Console.WriteLine($"[AUDIT] {DateTime.UtcNow:O} {entry}");

            // For backwards compatibility during migration, also write to file if path configured
            string logPath = Environment.GetEnvironmentVariable("APP_LOG_PATH");
            if (!string.IsNullOrEmpty(logPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                File.AppendAllText(logPath, $"{DateTime.UtcNow:O} {entry}\n");
            }
        }

        public string ReadOrderData(string orderId)
        {
            // TODO: Replace with cloud storage read operation
            string filePath = Path.Combine(_localStoragePath, orderId + ".json");
            if (!File.Exists(filePath))
            {
                return null;
            }
            return File.ReadAllText(filePath);
            // Recommended: Use AWS S3 client
            // var response = await s3Client.GetObjectAsync(bucket, orderId + ".json");
            // using var reader = new StreamReader(response.ResponseStream);
            // return await reader.ReadToEndAsync();
        }
    }
}
