// =============================================================================
// RULE ID   : cr-dotnet-0011
// RULE NAME : Hard-coded Service URLs
// CATEGORY  : Configuration
// DESCRIPTION: Application contains hard-coded URLs pointing to environment-specific
//              services, APIs, or endpoints embedded in code or configuration.
//              Prevents portability across cloud environments.
// =============================================================================
using System.Net.Http;
using System.Threading.Tasks;

namespace SyntheticLegacyApp.Configuration
{
    public class HardCodedServiceUrls
    {
        // VIOLATION cr-dotnet-0011: Production service URLs hard-coded in constants
        private const string PaymentServiceUrl   = "http://payments.corp.internal:8080/api/";
        private const string InventoryServiceUrl = "http://inventory.corp.internal/svc/";
        private const string AuthServiceUrl      = "https://auth.corp.internal/oauth2/token";
        private const string ReportingApiUrl     = "http://10.10.20.45:9090/reports/"; // IP address

        public async Task<string> GetPaymentStatus(string paymentId)
        {
            // VIOLATION cr-dotnet-0011: Hard-coded URL - requires code change per environment
            using (var client = new HttpClient())
                return await client.GetStringAsync(PaymentServiceUrl + "status/" + paymentId);
        }

        public string BuildInventoryEndpoint(string productId)
        {
            return InventoryServiceUrl + "product/" + productId; // VIOLATION cr-dotnet-0011
        }
    }
}
