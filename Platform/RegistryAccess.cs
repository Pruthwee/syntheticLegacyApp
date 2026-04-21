// =============================================================================
// RULE ID   : cr-dotnet-0040
// RULE NAME : Registry Access
// CATEGORY  : Platform
// DESCRIPTION: Application accesses Windows Registry using Microsoft.Win32.Registry
//              or RegistryKey classes for configuration or data storage. Windows
//              Registry doesn't exist on non-Windows cloud platforms, causing
//              registry access failures.
// =============================================================================

using System;
using Microsoft.Win32;

namespace SyntheticLegacyApp.Platform
{
    public class RegistryConfigProvider
    {
        // VIOLATION cr-dotnet-0040: Reading application settings from HKLM registry hive
        public string GetInstallPath()
        {
            using (RegistryKey key = Registry.LocalMachine
                .OpenSubKey(@"SOFTWARE\Trianz\LegacyApp"))
            {
                return key?.GetValue("InstallPath")?.ToString() ?? @"C:\LegacyApp";
            }
        }

        // VIOLATION cr-dotnet-0040: Reading license key from Windows Registry
        public string GetLicenseKey()
        {
            using (RegistryKey key = Registry.CurrentUser
                .OpenSubKey(@"SOFTWARE\Trianz\LegacyApp\License"))
            {
                if (key == null) throw new InvalidOperationException("License key not found in registry.");
                return key.GetValue("Key")?.ToString();
            }
        }

        // VIOLATION cr-dotnet-0040: Writing runtime config values to registry
        public void SetLastRunTime(DateTime runTime)
        {
            using (RegistryKey key = Registry.LocalMachine
                .CreateSubKey(@"SOFTWARE\Trianz\LegacyApp\Runtime"))
            {
                key.SetValue("LastRun", runTime.ToString("o"));
            }
        }

        // VIOLATION cr-dotnet-0040: Querying OS version from registry
        public string GetWindowsProductName()
        {
            using (RegistryKey key = Registry.LocalMachine
                .OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
            {
                return key?.GetValue("ProductName")?.ToString() ?? "Unknown";
            }
        }

        // VIOLATION cr-dotnet-0040: Enumerating sub-keys to discover installed plugins
        public string[] GetInstalledPlugins()
        {
            using (RegistryKey key = Registry.LocalMachine
                .OpenSubKey(@"SOFTWARE\Trianz\LegacyApp\Plugins"))
            {
                return key?.GetSubKeyNames() ?? Array.Empty<string>();
            }
        }
    }
}
