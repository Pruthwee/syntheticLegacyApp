// =============================================================================
// RULE ID   : cr-dotnet-0011
// RULE NAME : Hard-coded Service URLs
// CATEGORY  : Configuration
// DESCRIPTION: FIXED - URLs now loaded from AWS Systems Manager Parameter Store
// =============================================================================
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace SyntheticLegacyApp.Configuration
{
    public class HardCodedServiceUrls
    {
        private readonly IAmazonSimpleSystemsManagement _ssmClient;
        private string _paymentServiceUrl;
        private string _inventoryServiceUrl;
        private string _authServiceUrl;
        private string _reportingApiUrl;

        public HardCodedServiceUrls(IAmazonSimpleSystemsManagement ssmClient = null)
        {
            _ssmClient = ssmClient ?? new AmazonSimpleSystemsManagementClient();
            InitializeUrlsAsync().GetAwaiter().GetResult();
        }

        private async Task InitializeUrlsAsync()
        {
            _paymentServiceUrl = await GetParameterAsync("/app/services/payment-url");
            _inventoryServiceUrl = await GetParameterAsync("/app/services/inventory-url");
            _authServiceUrl = await GetParameterAsync("/app/services/auth-url");
            _reportingApiUrl = await GetParameterAsync("/app/services/reporting-url");
        }

        private async Task<string> GetParameterAsync(string parameterName)
        {
            try
            {
                var request = new GetParameterRequest
                {
                    Name = parameterName,
                    WithDecryption = true
                };
                var response = await _ssmClient.GetParameterAsync(request);
                return response.Parameter.Value;
            }
            catch (Exception ex)
            {
                // Fallback to environment variable if SSM parameter not found
                var envVarName = parameterName.Replace("/app/services/", "").Replace("-", "_").ToUpper();
                return Environment.GetEnvironmentVariable(envVarName) ?? 
                       throw new InvalidOperationException($"Configuration not found: {parameterName}", ex);
            }
        }

        public async Task<string> GetPaymentStatus(string paymentId)
        {
            // FIXED: URL loaded from AWS Systems Manager Parameter Store
            using (var client = new HttpClient())
                return await client.GetStringAsync(_paymentServiceUrl + "status/" + paymentId);
        }

        public string BuildInventoryEndpoint(string productId)
        {
            // FIXED: URL loaded from AWS Systems Manager Parameter Store
            return _inventoryServiceUrl + "product/" + productId;
        }
    }
}
