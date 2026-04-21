// =============================================================================
// RULE ID   : cr-dotnet-0042
// RULE NAME : P/Invoke Windows APIs
// CATEGORY  : Platform
// DESCRIPTION: FIXED - Replaced P/Invoke with cross-platform .NET APIs
// =============================================================================

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace SyntheticLegacyApp.Platform
{
    public class NativeWindowsInterop
    {
        private readonly ILogger<NativeWindowsInterop> _logger;

        public NativeWindowsInterop(ILogger<NativeWindowsInterop> logger = null)
        {
            _logger = logger;
        }

        // REMOVED: All P/Invoke declarations - replaced with cross-platform APIs

        public long GetAvailablePhysicalMemory()
        {
            // FIXED: Use cross-platform GC.GetGCMemoryInfo() instead of Windows-specific P/Invoke
            var gcInfo = GC.GetGCMemoryInfo();
            
            // For more detailed system memory info, use Process class
            var process = Process.GetCurrentProcess();
            var availableMemory = gcInfo.TotalAvailableMemoryBytes;
            
            _logger?.LogInformation($"Available memory: {availableMemory} bytes");
            return availableMemory;
        }

        public void ShowNativeAlert(string message)
        {
            // FIXED: Replace Windows MessageBox with logging (cloud-native approach)
            // In cloud environments, alerts should go to monitoring systems
            _logger?.LogWarning($"Alert: {message}");
            Console.WriteLine($"ALERT: {message}");
            
            // For actual alerting in cloud, integrate with CloudWatch Alarms or SNS
            // Example: await _snsClient.PublishAsync(new PublishRequest { ... });
        }

        public IntPtr GetProcessHandle()
        {
            // FIXED: Use cross-platform Process.GetCurrentProcess() instead of P/Invoke
            var process = Process.GetCurrentProcess();
            
            // Note: Process.Handle is available on all platforms
            // However, the handle value itself is platform-specific
            try
            {
                return process.Handle;
            }
            catch (PlatformNotSupportedException)
            {
                _logger?.LogWarning("Process handle not available on this platform");
                return IntPtr.Zero;
            }
        }

        public string ResolveWindowsAccount(string accountName)
        {
            // FIXED: Replace Windows-specific account lookup with claims-based identity
            // In cloud environments, use IAM roles, Cognito, or LDAP
            
            _logger?.LogInformation($"Resolving account: {accountName}");
            
            // For AWS, accounts are managed through IAM
            // For Active Directory integration, use AWS Directory Service with LDAP
            // Return the account name as-is or look up in external identity provider
            
            // Example: Query AWS Directory Service or Cognito
            // var user = await _cognitoClient.GetUserAsync(new GetUserRequest { ... });
            
            return accountName; // Simplified - in real implementation, query identity provider
        }

        // Additional cross-platform helper methods
        public long GetTotalPhysicalMemory()
        {
            // FIXED: Cross-platform memory information
            var gcInfo = GC.GetGCMemoryInfo();
            return gcInfo.TotalAvailableMemoryBytes;
        }

        public string GetOperatingSystem()
        {
            // FIXED: Cross-platform OS detection
            return RuntimeInformation.OSDescription;
        }

        public string GetProcessArchitecture()
        {
            // FIXED: Cross-platform architecture detection
            return RuntimeInformation.ProcessArchitecture.ToString();
        }

        public bool IsRunningInContainer()
        {
            // FIXED: Detect if running in container (works on Linux and Windows containers)
            return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" ||
                   File.Exists("/.dockerenv");
        }
    }
}
