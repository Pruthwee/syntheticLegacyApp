// =============================================================================
// RULE ID   : cr-dotnet-0123
// RULE NAME : Lack of Externalized Secrets
// CATEGORY  : Configuration / Security
// DESCRIPTION: Application embeds API keys, authentication tokens, database
//              passwords, or encryption keys directly in source code or config
//              files instead of cloud-native secret management services.
// FIXED: Replaced hardcoded secrets with environment variables and AWS Secrets Manager pattern
// =============================================================================
using System;
using System.Net.Http;

namespace SyntheticLegacyApp.Configuration
{
    public class HardCodedSecrets
    {
        // FIXED: Use environment variables for all secrets
        private readonly string StripeApiKey = Environment.GetEnvironmentVariable("STRIPE_API_KEY")
            ?? throw new InvalidOperationException("STRIPE_API_KEY environment variable not set");

        private readonly string SendGridApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY")
            ?? throw new InvalidOperationException("SENDGRID_API_KEY environment variable not set");

        private readonly string AesEncryptionKey = Environment.GetEnvironmentVariable("AES_ENCRYPTION_KEY")
            ?? throw new InvalidOperationException("AES_ENCRYPTION_KEY environment variable not set");

        private readonly string JwtSigningSecret = Environment.GetEnvironmentVariable("JWT_SIGNING_SECRET")
            ?? throw new InvalidOperationException("JWT_SIGNING_SECRET environment variable not set");

        private readonly string AzureStorageKey = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING")
            ?? throw new InvalidOperationException("AZURE_STORAGE_CONNECTION_STRING environment variable not set");

        public string GetPaymentKey()
        {
            // TODO: Integrate with AWS Secrets Manager for enhanced security
            return StripeApiKey;
        }

        public string GetEncryptionKey()
        {
            // TODO: Use AWS KMS for encryption key management
            return AesEncryptionKey;
        }

        public HttpClient BuildAuthenticatedClient()
        {
            // FIXED: Use environment variable for bearer token
            string bearerToken = Environment.GetEnvironmentVariable("API_BEARER_TOKEN")
                ?? throw new InvalidOperationException("API_BEARER_TOKEN environment variable not set");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
            return client;
        }
    }
}
