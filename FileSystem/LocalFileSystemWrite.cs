// =============================================================================
// RULE ID   : cr-dotnet-0002
// RULE NAME : Local File System Write Operations
// CATEGORY  : File System
// DESCRIPTION: FIXED - Replaced local file writes with Amazon S3
// =============================================================================
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace SyntheticLegacyApp.FileSystem
{
    public class LocalFileSystemWrite
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _s3Bucket;
        private readonly string _orderPrefix = "orders/";
        private readonly string _sessionPrefix = "sessions/";
        private readonly string _auditLogKey = "logs/audit.log";

        public LocalFileSystemWrite(IAmazonS3 s3Client = null)
        {
            _s3Client = s3Client ?? new AmazonS3Client();
            _s3Bucket = Environment.GetEnvironmentVariable("S3_BUCKET") ?? 
                       throw new InvalidOperationException("S3_BUCKET environment variable not set");
        }

        public async Task SaveOrderDataAsync(string orderId, string serializedOrder)
        {
            // FIXED: Write to S3 instead of local file system
            string s3Key = $"{_orderPrefix}{orderId}.json";
            
            var request = new PutObjectRequest
            {
                BucketName = _s3Bucket,
                Key = s3Key,
                ContentBody = serializedOrder,
                ContentType = "application/json",
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
            };
            
            await _s3Client.PutObjectAsync(request);
        }

        public void SaveOrderData(string orderId, string serializedOrder)
        {
            SaveOrderDataAsync(orderId, serializedOrder).GetAwaiter().GetResult();
        }

        public async Task SaveUserSessionAsync(string sessionId, byte[] sessionData)
        {
            // FIXED: Write session data to S3 instead of local disk
            string s3Key = $"{_sessionPrefix}{sessionId}.dat";
            
            using (var ms = new MemoryStream(sessionData))
            {
                var request = new PutObjectRequest
                {
                    BucketName = _s3Bucket,
                    Key = s3Key,
                    InputStream = ms,
                    ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
                    // Set expiration for session data
                    TagSet = new System.Collections.Generic.List<Tag>
                    {
                        new Tag { Key = "Type", Value = "Session" },
                        new Tag { Key = "ExpiresAfter", Value = "24h" }
                    }
                };
                
                await _s3Client.PutObjectAsync(request);
            }
        }

        public void SaveUserSession(string sessionId, byte[] sessionData)
        {
            SaveUserSessionAsync(sessionId, sessionData).GetAwaiter().GetResult();
        }

        public async Task AppendAuditLogAsync(string entry)
        {
            // FIXED: Append to S3 object (or use CloudWatch Logs for better audit logging)
            // Note: S3 doesn't support true append, so we read-modify-write
            // For production, consider using CloudWatch Logs or DynamoDB for audit logs
            
            try
            {
                // Try to read existing log
                var getRequest = new GetObjectRequest
                {
                    BucketName = _s3Bucket,
                    Key = _auditLogKey
                };
                
                string existingContent = "";
                try
                {
                    using (var response = await _s3Client.GetObjectAsync(getRequest))
                    using (var reader = new StreamReader(response.ResponseStream))
                    {
                        existingContent = await reader.ReadToEndAsync();
                    }
                }
                catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // File doesn't exist yet, start fresh
                }
                
                // Append new entry
                string newContent = existingContent + entry + "\n";
                
                // Write back to S3
                var putRequest = new PutObjectRequest
                {
                    BucketName = _s3Bucket,
                    Key = _auditLogKey,
                    ContentBody = newContent,
                    ContentType = "text/plain"
                };
                
                await _s3Client.PutObjectAsync(putRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to append audit log: {ex.Message}");
                // Consider fallback to CloudWatch Logs
            }
        }

        public void AppendAuditLog(string entry)
        {
            AppendAuditLogAsync(entry).GetAwaiter().GetResult();
        }

        public async Task<string> ReadOrderDataAsync(string orderId)
        {
            // FIXED: Read from S3 instead of local file system
            string s3Key = $"{_orderPrefix}{orderId}.json";
            
            var request = new GetObjectRequest
            {
                BucketName = _s3Bucket,
                Key = s3Key
            };
            
            using (var response = await _s3Client.GetObjectAsync(request))
            using (var reader = new StreamReader(response.ResponseStream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public string ReadOrderData(string orderId)
        {
            return ReadOrderDataAsync(orderId).GetAwaiter().GetResult();
        }
    }
}
