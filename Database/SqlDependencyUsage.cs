// =============================================================================
// RULE ID   : cr-dotnet-0015
// RULE NAME : SqlDependency Usage
// CATEGORY  : Database
// DESCRIPTION: Application uses SqlDependency or SqlCacheDependency for database
//              change notifications. These require specific SQL Server configurations
//              and do not work reliably in cloud database services.
// =============================================================================
using System.Data.SqlClient;
using System.Web.Caching;

namespace SyntheticLegacyApp.Database
{
    public class SqlDependencyUsage
    {
        private readonly string _cs = "Server=db;Database=AppDB;User Id=app;Password=p;";

        public void StartChangeNotifications()
        {
            // VIOLATION cr-dotnet-0015: SqlDependency.Start requires SQL Server Service Broker
            SqlDependency.Start(_cs);
        }

        public void MonitorProductChanges()
        {
            using (var conn = new SqlConnection(_cs))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT ProductId, StockLevel FROM dbo.Products", conn);
                // VIOLATION cr-dotnet-0015: SqlDependency wired to command
                var dep = new SqlDependency(cmd);
                dep.OnChange += (s, e) => { /* refresh cache */ };
                cmd.ExecuteReader();
            }
        }

        public CacheDependency GetCacheDependency()
        {
            // VIOLATION cr-dotnet-0015: SqlCacheDependency tied to SQL Server infrastructure
            return new SqlCacheDependency("AppDB", "Products");
        }
    }
}
