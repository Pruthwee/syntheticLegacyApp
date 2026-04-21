// =============================================================================
// RULE ID   : cr-dotnet-0009
// RULE NAME : Hard-coded Connection Strings
// CATEGORY  : Configuration
// DESCRIPTION: Application contains database connection strings directly embedded
//              in source code. Creates security vulnerabilities, prevents
//              environment-specific config, and violates cloud credential management.
// =============================================================================
using System.Data.SqlClient;

namespace SyntheticLegacyApp.Configuration
{
    public class HardCodedConnectionStrings
    {
        // VIOLATION cr-dotnet-0009: Connection string with credentials in source
        private const string PrimaryDb =
            "Server=prod-db01.corp.internal;Database=SyntheticAppDB;" +
            "User Id=sa;Password=Passw0rd!2024;";

        private const string ReportingDb =
            "Data Source=reports-sql.corp.local;Initial Catalog=ReportsDB;" +
            "Integrated Security=False;Uid=reports_svc;Pwd=R3p0rtsP@ss;";

        public SqlConnection GetPrimaryConnection()
        {
            return new SqlConnection(PrimaryDb); // VIOLATION cr-dotnet-0009
        }

        public void ExecuteQuery(string sql)
        {
            // VIOLATION cr-dotnet-0009: Inline connection string with credentials
            using (var conn = new SqlConnection(
                "Server=legacy-db.corp.net;Database=LegacyDB;User Id=app_user;Password=L3g@cy!;"))
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn)) cmd.ExecuteNonQuery();
            }
        }
    }
}
