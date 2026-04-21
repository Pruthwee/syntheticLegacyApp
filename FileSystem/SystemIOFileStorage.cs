// =============================================================================
// RULE ID   : cr-dotnet-0003
// RULE NAME : System.IO.File for Data Storage
// CATEGORY  : File System
// DESCRIPTION: Application uses System.IO.File API for persistent data storage
//              instead of cloud storage services. Limits scalability and violates
//              cloud-native storage patterns where data should be externalized.
// =============================================================================
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace SyntheticLegacyApp.FileSystem
{
    public class SystemIOFileStorage
    {
        private readonly string _dataRoot = @"C:\AppData\SyntheticApp";

        // VIOLATION cr-dotnet-0003: Using File API as persistent data store
        public void StoreProduct(string productId, string productJson)
        {
            string path = Path.Combine(_dataRoot, "products", productId + ".json");
            File.WriteAllText(path, productJson);
        }

        public string RetrieveProduct(string productId)
        {
            string path = Path.Combine(_dataRoot, "products", productId + ".json");
            return File.Exists(path) ? File.ReadAllText(path) : null;
        }

        // VIOLATION cr-dotnet-0003: File.Copy used to simulate record archival
        public void ArchiveProduct(string productId)
        {
            string current  = Path.Combine(_dataRoot, "products", productId + ".json");
            string archived = Path.Combine(_dataRoot, "archive", productId + "_archived.json");
            File.Copy(current, archived, overwrite: true);
            File.Delete(current);
        }

        public List<string> ListAllProducts()
        {
            string dir = Path.Combine(_dataRoot, "products");
            return new List<string>(Directory.GetFiles(dir, "*.json"));
        }
    }
}
