// =============================================================================
// RULE ID   : cr-dotnet-0011
// RULE NAME : Hard-coded Service URLs
// CATEGORY  : Configuration
// DESCRIPTION: Application contains hard-coded URLs pointing to environment-specific
//              services, APIs, or endpoints embedded in code or configuration.
//              Prevents portability across cloud environments.
// FIXED: Replaced hardcoded URLs with environment variables
// =============================================================================
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SyntheticLegacyApp.Configuration
{
    public class HardCodedServiceUrls
    {
        // FIXED: Use environment variables for service URLs
        private readonly string PaymentServiceUrl = Environment.GetEnvironmentVariable("PAYMENT_SERVICE_URL")
            ?? throw new InvalidOperationException("PAYMENT_SERVICE_URL environment variable not set");

        private readonly string InventoryServiceUrl = Environment.GetEnvironmentVariable("INVENTORY_SERVICE_URL")
            ?? throw new InvalidOperationException("INVENTORY_SERVICE_URL environment variable not set");

        private readonly string AuthServiceUrl = Environment.GetEnvironmentVariable("AUTH_SERVICE_URL")
            ?? throw new InvalidOperationException("AUTH_SERVICE_URL environment variable not set");

        private readonly string ReportingApiUrl = Environment.GetEnvironmentVariable("REPORTING_API_URL")
            ?? throw new InvalidOperationException("REPORTING_API_URL environment variable not set");

        public async Task<string> GetPaymentStatus(string paymentId)
        {
            // FIXED: Use environment-configured URL with proper URI construction
            using (var client = new HttpClient())
            {
                var uri = new Uri(new Uri(PaymentServiceUrl), $"status/{paymentId}");
                return await client.GetStringAsync(uri);
            }
        }

        public string BuildInventoryEndpoint(string productId)
        {
            // FIXED: Use environment variable with proper URI construction
            var baseUri = new Uri(InventoryServiceUrl);
            return new Uri(baseUri, $"product/{productId}").ToString();
        }
    }
}
