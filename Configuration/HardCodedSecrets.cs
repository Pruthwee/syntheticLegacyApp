// =============================================================================
// RULE ID   : cr-dotnet-0123
// RULE NAME : Lack of Externalized Secrets
// CATEGORY  : Configuration / Security
// DESCRIPTION: Application embeds API keys, authentication tokens, database
//              passwords, or encryption keys directly in source code or config
//              files instead of cloud-native secret management services.
// =============================================================================
using System.Net.Http;

namespace SyntheticLegacyApp.Configuration
{
    public class HardCodedSecrets
    {
        // VIOLATION cr-dotnet-0123: API keys embedded in source code
        private const string StripeApiKey    = "sk_live_4eC39HqLyjWDarjtT1zdp7dc";
        private const string SendGridApiKey  = "SG.xxxxxxxxxxxxxxxxxxxxxxxxx.yyyyyyyyyyyyyyyyyy";
        private const string AesEncryptionKey = "MySuperSecretKey1";
        private const string JwtSigningSecret = "jwt-secret-do-not-share-2024";

        private const string AzureStorageKey =
            "DefaultEndpointsProtocol=https;AccountName=stgsynthetic;" +
            "AccountKey=AAABBBCCC111222333===;EndpointSuffix=core.windows.net";

        public string GetPaymentKey()   => StripeApiKey;     // VIOLATION cr-dotnet-0123
        public string GetEncryptionKey() => AesEncryptionKey; // VIOLATION cr-dotnet-0123

        public HttpClient BuildAuthenticatedClient()
        {
            var client = new HttpClient();
            // VIOLATION cr-dotnet-0123: Bearer token hard-coded inline
            client.DefaultRequestHeaders.Add("Authorization",
                "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.hardcoded_token");
            return client;
        }
    }
}
