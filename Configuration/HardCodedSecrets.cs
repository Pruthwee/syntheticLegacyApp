// =============================================================================
// RULE ID   : cr-dotnet-0123
// RULE NAME : Lack of Externalized Secrets
// CATEGORY  : Configuration / Security
// DESCRIPTION: FIXED - Migrated secrets to AWS Secrets Manager with SDK integration
// =============================================================================
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json.Linq;

namespace SyntheticLegacyApp.Configuration
{
    public class HardCodedSecrets
    {
        private readonly IAmazonSecretsManager _secretsManager;
        private string _stripeApiKey;
        private string _sendGridApiKey;
        private string _aesEncryptionKey;
        private string _jwtSigningSecret;
        private string _azureStorageKey;

        public HardCodedSecrets(IAmazonSecretsManager secretsManager = null)
        {
            _secretsManager = secretsManager ?? new AmazonSecretsManagerClient();
            InitializeSecretsAsync().GetAwaiter().GetResult();
        }

        private async Task InitializeSecretsAsync()
        {
            // FIXED: Load all secrets from AWS Secrets Manager
            _stripeApiKey = await GetSecretValueAsync("api-keys/stripe");
            _sendGridApiKey = await GetSecretValueAsync("api-keys/sendgrid");
            _aesEncryptionKey = await GetSecretValueAsync("encryption/aes-key");
            _jwtSigningSecret = await GetSecretValueAsync("auth/jwt-signing-secret");
            _azureStorageKey = await GetSecretValueAsync("storage/azure-connection");
        }

        private async Task<string> GetSecretValueAsync(string secretName)
        {
            try
            {
                var request = new GetSecretValueRequest
                {
                    SecretId = secretName
                };
                
                var response = await _secretsManager.GetSecretValueAsync(request);
                
                if (!string.IsNullOrEmpty(response.SecretString))
                {
                    // Try to parse as JSON and extract value
                    try
                    {
                        var json = JObject.Parse(response.SecretString);
                        // Look for common key names
                        if (json["value"] != null) return json["value"].ToString();
                        if (json["apiKey"] != null) return json["apiKey"].ToString();
                        if (json["key"] != null) return json["key"].ToString();
                        if (json["secret"] != null) return json["secret"].ToString();
                        
                        // If no standard key found, return first property value
                        foreach (var prop in json.Properties())
                        {
                            return prop.Value.ToString();
                        }
                    }
                    catch
                    {
                        // Not JSON, return as-is
                    }
                    
                    return response.SecretString;
                }
                
                throw new InvalidOperationException($"Secret {secretName} is empty");
            }
            catch (Exception ex)
            {
                // Fallback to environment variable
                var envVarName = secretName.Replace("/", "_").Replace("-", "_").ToUpper();
                var envValue = Environment.GetEnvironmentVariable(envVarName);
                
                if (string.IsNullOrEmpty(envValue))
                {
                    throw new InvalidOperationException(
                        $"Failed to retrieve secret {secretName} from Secrets Manager and no environment variable {envVarName} found", ex);
                }
                
                return envValue;
            }
        }

        public string GetPaymentKey()
        {
            // FIXED: Return secret loaded from AWS Secrets Manager
            return _stripeApiKey;
        }

        public string GetEncryptionKey()
        {
            // FIXED: Return secret loaded from AWS Secrets Manager
            return _aesEncryptionKey;
        }

        public string GetJwtSigningSecret()
        {
            // FIXED: Return secret loaded from AWS Secrets Manager
            return _jwtSigningSecret;
        }

        public HttpClient BuildAuthenticatedClient()
        {
            // FIXED: Use token from Secrets Manager instead of hard-coded value
            var client = new HttpClient();
            
            // Get bearer token from Secrets Manager
            var token = GetSecretValueAsync("auth/bearer-token").GetAwaiter().GetResult();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            
            return client;
        }

        public async Task<HttpClient> BuildAuthenticatedClientAsync()
        {
            var client = new HttpClient();
            var token = await GetSecretValueAsync("auth/bearer-token");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            return client;
        }

        // Additional helper methods
        public string GetSendGridApiKey() => _sendGridApiKey;
        public string GetAzureStorageConnectionString() => _azureStorageKey;
    }
}
