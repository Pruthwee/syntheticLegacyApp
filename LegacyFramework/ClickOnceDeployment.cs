// =============================================================================
// RULE ID   : cr-dotnet-0048
// RULE NAME : ClickOnce Deployment
// CATEGORY  : LegacyFramework
// DESCRIPTION: Application uses ClickOnce deployment technology for application
//              distribution and updates. ClickOnce assumes desktop deployment
//              scenarios that don't apply to cloud-hosted applications and services.
// =============================================================================

using System;
using System.Deployment.Application;
using System.Reflection;

namespace SyntheticLegacyApp.LegacyFramework
{
    public class ClickOnceUpdateManager
    {
        // VIOLATION cr-dotnet-0048: ApplicationDeployment.IsNetworkDeployed — ClickOnce only
        public bool IsRunningAsClickOnce => ApplicationDeployment.IsNetworkDeployed;

        public Version GetCurrentVersion()
        {
            if (!ApplicationDeployment.IsNetworkDeployed)
                return Assembly.GetExecutingAssembly().GetName().Version;

            // VIOLATION cr-dotnet-0048: CurrentDeployment.CurrentVersion — ClickOnce manifest version
            return ApplicationDeployment.CurrentDeployment.CurrentVersion;
        }

        // VIOLATION cr-dotnet-0048: CheckForUpdate relies on ClickOnce manifest server
        public bool CheckForUpdate()
        {
            if (!ApplicationDeployment.IsNetworkDeployed)
            {
                Console.WriteLine("Not running as ClickOnce deployment.");
                return false;
            }

            UpdateCheckInfo info = ApplicationDeployment.CurrentDeployment
                .CheckForDetailedUpdate();

            if (info.UpdateAvailable)
            {
                Console.WriteLine($"Update available: {info.AvailableVersion}. " +
                    $"Required: {info.IsUpdateRequired}");
                return true;
            }

            return false;
        }

        // VIOLATION cr-dotnet-0048: Update() triggers ClickOnce self-update — desktop-only mechanism
        public void PerformUpdate()
        {
            if (!ApplicationDeployment.IsNetworkDeployed) return;

            var deployment = ApplicationDeployment.CurrentDeployment;

            deployment.UpdateCompleted += (s, e) =>
            {
                if (e.Error != null)
                {
                    Console.WriteLine($"ClickOnce update failed: {e.Error.Message}");
                    return;
                }
                Console.WriteLine("ClickOnce update complete. Restart required.");
            };

            // VIOLATION cr-dotnet-0048: Async ClickOnce update — meaningless in server/cloud context
            deployment.UpdateAsync();
        }

        // VIOLATION cr-dotnet-0048: Reading ClickOnce activation URI — desktop-only concept
        public string GetActivationUri()
        {
            if (!ApplicationDeployment.IsNetworkDeployed) return string.Empty;

            return ApplicationDeployment.CurrentDeployment
                .ActivationUri?.ToString() ?? string.Empty;
        }

        // VIOLATION cr-dotnet-0048: DataDirectory used for per-user ClickOnce data storage
        public string GetClickOnceDataPath()
        {
            return ApplicationDeployment.IsNetworkDeployed
                ? ApplicationDeployment.CurrentDeployment.DataDirectory
                : AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
