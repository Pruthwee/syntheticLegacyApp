// =============================================================================
// RULE ID   : cr-dotnet-0053
// RULE NAME : Environment.MachineName
// CATEGORY  : Platform
// DESCRIPTION: Application uses Environment.MachineName property to identify the
//              hosting machine or for business logic decisions. In cloud environments,
//              machine names are ephemeral and randomly generated, making
//              machine name-based logic unreliable.
// =============================================================================

using System;
using System.Collections.Generic;

namespace SyntheticLegacyApp.Platform
{
    public class NodeIdentityService
    {
        // VIOLATION cr-dotnet-0053: Environment.MachineName used as stable node identifier
        public string GetNodeId()
        {
            return Environment.MachineName; // unreliable in cloud — ephemeral pod hostnames
        }

        // VIOLATION cr-dotnet-0053: MachineName embedded in audit records
        public Dictionary<string, string> BuildAuditRecord(string action, string user)
        {
            return new Dictionary<string, string>
            {
                ["action"]    = action,
                ["user"]      = user,
                ["server"]    = Environment.MachineName, // VIOLATION cr-dotnet-0053
                ["timestamp"] = DateTime.UtcNow.ToString("o")
            };
        }

        // VIOLATION cr-dotnet-0053: Primary/secondary election based on machine name
        public bool IsPrimaryNode()
        {
            string expected = "PROD-APP-01"; // hard-coded machine name expectation
            return string.Equals(Environment.MachineName, expected,
                StringComparison.OrdinalIgnoreCase);
        }

        // VIOLATION cr-dotnet-0053: Config selection keyed by machine name
        public string GetEnvironmentConfig()
        {
            string machine = Environment.MachineName.ToUpper();

            return machine switch
            {
                "DEV-BOX-001"  => "development",
                "QA-SERVER-01" => "qa",
                "PROD-APP-01"  => "production",
                _              => "unknown"
            };
        }
    }

    public class LicenseValidator
    {
        // VIOLATION cr-dotnet-0053: License tied to machine name — breaks on pod restart
        private readonly HashSet<string> _licensedMachines = new HashSet<string>
        {
            "PROD-APP-01", "PROD-APP-02", "PROD-APP-03"
        };

        public bool IsLicensed()
        {
            // VIOLATION cr-dotnet-0053: Machine name as license anchor — fails on ephemeral hosts
            return _licensedMachines.Contains(Environment.MachineName);
        }

        public string GenerateMachineFingerprint()
        {
            // VIOLATION cr-dotnet-0053: Fingerprint includes machine name — changes on pod replacement
            return $"{Environment.MachineName}_{Environment.ProcessorCount}_{Environment.OSVersion}";
        }
    }
}
