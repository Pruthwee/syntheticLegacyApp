// =============================================================================
// RULE ID   : cr-dotnet-0012
// RULE NAME : Machine.config Dependencies
// CATEGORY  : Configuration
// DESCRIPTION: Application depends on machine.config for machine-wide configuration.
//              Machine.config is a Windows/.NET Framework concept that does not
//              exist in containerized environments, causing configuration failures.
// =============================================================================
using System.Configuration;
using System.Web.Configuration;

namespace SyntheticLegacyApp.Configuration
{
    public class MachineConfigDeps
    {
        public string GetMachineLevelSetting(string key)
        {
            // VIOLATION cr-dotnet-0012: Opening machine-level configuration
            var machineConfig = ConfigurationManager.OpenMachineConfiguration();
            return machineConfig.AppSettings.Settings[key]?.Value;
        }

        public void ConfigureFromMachine()
        {
            // VIOLATION cr-dotnet-0012: Machine.config section - unavailable in containers
            Configuration machineConfig = WebConfigurationManager.OpenMachineConfiguration();
            var section = (CompilationSection)machineConfig.GetSection("system.web/compilation");
        }
    }
}
