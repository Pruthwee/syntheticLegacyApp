// =============================================================================
// RULE ID   : cr-dotnet-0009
// RULE NAME : Hard-coded Connection Strings
// CATEGORY  : Configuration
// DESCRIPTION: Application contains database connection strings directly embedded
//              in source code. Creates security vulnerabilities, prevents
//              environment-specific config, and violates cloud credential management.
// FIXED: Replaced hardcoded connection strings with environment variables
// =============================================================================
using System;
using System.Data.SqlClient;

namespace SyntheticLegacyApp.Configuration
{
    public class HardCodedConnectionStrings
    {
        // FIXED: Use environment variables for connection strings
        private readonly string PrimaryDb = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
            ?? throw new InvalidOperationException("DATABASE_CONNECTION_STRING environment variable not set");

        private readonly string ReportingDb = Environment.GetEnvironmentVariable("REPORTING_DB_CONNECTION_STRING")
            ?? throw new InvalidOperationException("REPORTING_DB_CONNECTION_STRING environment variable not set");

        public SqlConnection GetPrimaryConnection()
        {
            // TODO: Consider using AWS RDS Proxy for connection pooling
            // TODO: For sensitive data, integrate with AWS Secrets Manager
            return new SqlConnection(PrimaryDb);
        }

        public void ExecuteQuery(string sql)
        {
            // FIXED: Use environment variable for connection string
            string legacyDbConnectionString = Environment.GetEnvironmentVariable("LEGACY_DB_CONNECTION_STRING")
                ?? throw new InvalidOperationException("LEGACY_DB_CONNECTION_STRING environment variable not set");

            using (var conn = new SqlConnection(legacyDbConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
