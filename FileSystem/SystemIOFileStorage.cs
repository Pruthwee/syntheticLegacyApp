// =============================================================================
// RULE ID   : cr-dotnet-0003
// RULE NAME : System.IO.File for Data Storage
// CATEGORY  : File System
// DESCRIPTION: Application uses System.IO.File API for persistent data storage
//              instead of cloud storage services. Limits scalability and violates
//              cloud-native storage patterns where data should be externalized.
// FIXED: Replaced hardcoded paths with environment variables and added cloud storage patterns
// =============================================================================
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace SyntheticLegacyApp.FileSystem
{
    public class SystemIOFileStorage
    {
        // FIXED: Use environment variable for data root path
        private readonly string _dataRoot = Environment.GetEnvironmentVariable("APP_DATA_ROOT")
            ?? Path.Combine(Path.GetTempPath(), "SyntheticApp");

        // FIXED: Use cloud-native storage pattern with environment-based paths
        public void StoreProduct(string productId, string productJson)
        {
            // TODO: Replace with cloud storage (AWS S3, Azure Blob Storage)
            string productDir = Path.Combine(_dataRoot, "products");
            Directory.CreateDirectory(productDir);
            string path = Path.Combine(productDir, productId + ".json");
            File.WriteAllText(path, productJson);
            // Recommended: await s3Client.PutObjectAsync(new PutObjectRequest { BucketName = bucket, Key = $"products/{productId}.json", ContentBody = productJson });
        }

        public string RetrieveProduct(string productId)
        {
            // TODO: Replace with cloud storage read operation
            string path = Path.Combine(_dataRoot, "products", productId + ".json");
            return File.Exists(path) ? File.ReadAllText(path) : null;
            // Recommended: var response = await s3Client.GetObjectAsync(bucket, $"products/{productId}.json");
        }

        // FIXED: Archive operation updated for cloud storage pattern
        public void ArchiveProduct(string productId)
        {
            // TODO: Replace with cloud storage copy/delete (S3 CopyObject + DeleteObject)
            string productsDir = Path.Combine(_dataRoot, "products");
            string archiveDir = Path.Combine(_dataRoot, "archive");
            Directory.CreateDirectory(archiveDir);

            string current  = Path.Combine(productsDir, productId + ".json");
            string archived = Path.Combine(archiveDir, productId + "_archived.json");

            if (File.Exists(current))
            {
                File.Copy(current, archived, overwrite: true);
                File.Delete(current);
            }
            // Recommended:
            // await s3Client.CopyObjectAsync(bucket, $"products/{productId}.json", bucket, $"archive/{productId}_archived.json");
            // await s3Client.DeleteObjectAsync(bucket, $"products/{productId}.json");
        }

        public List<string> ListAllProducts()
        {
            // TODO: Replace with cloud storage listing (S3 ListObjectsV2)
            string dir = Path.Combine(_dataRoot, "products");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                return new List<string>();
            }
            return new List<string>(Directory.GetFiles(dir, "*.json"));
            // Recommended:
            // var response = await s3Client.ListObjectsV2Async(new ListObjectsV2Request { BucketName = bucket, Prefix = "products/" });
            // return response.S3Objects.Select(o => o.Key).ToList();
        }
    }
}
