// =============================================================================
// RULE ID   : cr-dotnet-0002
// RULE NAME : Local File System Write Operations
// CATEGORY  : File System
// DESCRIPTION: Application performs direct write operations to local file system
//              for data persistence. In cloud environments with ephemeral or
//              managed storage, local data may not persist across instance changes.
// =============================================================================
using System.IO;

namespace SyntheticLegacyApp.FileSystem
{
    public class LocalFileSystemWrite
    {
        private readonly string _localStoragePath = @"C:\AppData\SyntheticApp\Storage";

        public void SaveOrderData(string orderId, string serializedOrder)
        {
            // VIOLATION cr-dotnet-0002: Writing persistent data to local file system
            string filePath = Path.Combine(_localStoragePath, orderId + ".json");
            File.WriteAllText(filePath, serializedOrder);
        }

        public void SaveUserSession(string sessionId, byte[] sessionData)
        {
            // VIOLATION cr-dotnet-0002: Session data persisted to local disk
            string sessionFile = Path.Combine(_localStoragePath, "sessions", sessionId + ".dat");
            File.WriteAllBytes(sessionFile, sessionData);
        }

        public void AppendAuditLog(string entry)
        {
            // VIOLATION cr-dotnet-0002: Audit log written to local file path
            string logPath = @"C:\Logs\SyntheticApp\audit.log";
            File.AppendAllText(logPath, entry + "\n");
        }

        public string ReadOrderData(string orderId)
        {
            string filePath = Path.Combine(_localStoragePath, orderId + ".json");
            return File.ReadAllText(filePath);
        }
    }
}
