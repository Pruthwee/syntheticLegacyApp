// =============================================================================
// RULE ID   : cr-dotnet-0010
// RULE NAME : Web.config Transformations
// CATEGORY  : Configuration
// DESCRIPTION: Application relies on Web.config transformation files for
//              environment-specific configuration. Build-time transformations do
//              not work in cloud pipelines where config should be runtime-injectable.
// NOTE: Primary violation is in Web.Release.config / Web.Staging.config files.
//       This class shows the code that depends on transformed key values.
// =============================================================================
using System.Configuration;

namespace SyntheticLegacyApp.Configuration
{
    public class WebConfigTransformations
    {
        public string GetConnectionString()
        {
            // VIOLATION cr-dotnet-0010: Key expected to be overridden by Web.Release.config
            return ConfigurationManager.ConnectionStrings["AppDatabase"].ConnectionString;
        }

        public string GetTransformedServiceUrl()
        {
            // VIOLATION cr-dotnet-0010: appSettings value set by build-time transform
            return ConfigurationManager.AppSettings["ExternalServiceUrl"];
        }

        public bool IsFeatureEnabled(string featureKey)
        {
            // VIOLATION cr-dotnet-0010: Feature flags configured through build transforms
            string val = ConfigurationManager.AppSettings[featureKey];
            return bool.TryParse(val, out bool result) && result;
        }
    }
}
