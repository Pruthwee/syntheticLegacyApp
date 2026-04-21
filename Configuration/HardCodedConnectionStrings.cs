// =============================================================================
// RULE ID   : cr-dotnet-0009
// RULE NAME : Hard-coded Connection Strings
// CATEGORY  : Configuration
// DESCRIPTION: FIXED - Migrated connection strings to AWS Secrets Manager
// =============================================================================
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json.Linq;

namespace SyntheticLegacyApp.Configuration
{
    public class HardCodedConnectionStrings
    {
        private readonly IAmazonSecretsManager _secretsManager;
        private string _primaryDbConnectionString;
        private string _reportingDbConnectionString;

        public HardCodedConnectionStrings(IAmazonSecretsManager secretsManager = null)
        {
            _secretsManager = secretsManager ?? new AmazonSecretsManagerClient();
            InitializeConnectionStringsAsync().GetAwaiter().GetResult();
        }

        private async Task InitializeConnectionStringsAsync()
        {
            // FIXED: Load connection strings from AWS Secrets Manager
            _primaryDbConnectionString = await GetSecretAsync("database/primary");
            _reportingDbConnectionString = await GetSecretAsync("database/reporting");
        }

        private async Task<string> GetSecretAsync(string secretName)
        {
            try
            {
                var request = new GetSecretValueRequest
                {
                    SecretId = secretName
                };
                
                var response = await _secretsManager.GetSecretValueAsync(request);
                
                // Secret can be either string or JSON
                if (!string.IsNullOrEmpty(response.SecretString))
                {
                    // If it's JSON, extract the connection string
                    try
                    {
                        var json = JObject.Parse(response.SecretString);
                        if (json["connectionString"] != null)
                        {
                            return json["connectionString"].ToString();
                        }
                        // If it's a structured secret, build connection string
                        if (json["host"] != null && json["database"] != null)
                        {
                            return BuildConnectionString(json);
                        }
                        return response.SecretString;
                    }
                    catch
                    {
                        // Not JSON, return as-is
                        return response.SecretString;
                    }
                }
                
                throw new InvalidOperationException($"Secret {secretName} is empty");
            }
            catch (Exception ex)
            {
                // Fallback to environment variable if Secrets Manager fails
                var envVarName = secretName.Replace("/", "_").Replace("-", "_").ToUpper() + "_CONNECTION_STRING";
                var envValue = Environment.GetEnvironmentVariable(envVarName);
                
                if (string.IsNullOrEmpty(envValue))
                {
                    throw new InvalidOperationException(
                        $"Failed to retrieve secret {secretName} and no fallback environment variable {envVarName} found", ex);
                }
                
                return envValue;
            }
        }

        private string BuildConnectionString(JObject secretJson)
        {
            var host = secretJson["host"]?.ToString();
            var database = secretJson["database"]?.ToString();
            var username = secretJson["username"]?.ToString();
            var password = secretJson["password"]?.ToString();
            var port = secretJson["port"]?.ToString() ?? "1433";
            
            return $"Server={host},{port};Database={database};User Id={username};Password={password};Encrypt=True;TrustServerCertificate=False;";
        }

        public SqlConnection GetPrimaryConnection()
        {
            // FIXED: Connection string loaded from AWS Secrets Manager
            return new SqlConnection(_primaryDbConnectionString);
        }

        public async Task ExecuteQueryAsync(string sql)
        {
            // FIXED: Connection string from Secrets Manager
            using (var conn = new SqlConnection(_primaryDbConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public void ExecuteQuery(string sql)
        {
            ExecuteQueryAsync(sql).GetAwaiter().GetResult();
        }

        // Additional helper for reporting database
        public SqlConnection GetReportingConnection()
        {
            return new SqlConnection(_reportingDbConnectionString);
        }
    }
}
