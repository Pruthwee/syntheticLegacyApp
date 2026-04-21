// =============================================================================
// RULE ID   : cr-dotnet-0014
// RULE NAME : SQL Server Specific Features
// CATEGORY  : Database
// DESCRIPTION: Application uses SQL Server-specific features, T-SQL extensions,
//              or proprietary functionality tying the application to SQL Server.
//              Reduces database portability in cloud database service selection.
// =============================================================================
using System.Data;
using System.Data.SqlClient;

namespace SyntheticLegacyApp.Database
{
    public class SqlServerSpecific
    {
        private readonly string _cs = "Server=prod-db;Database=AppDB;User Id=app;Password=p@ss;";

        public void BulkInsertOrders(DataTable ordersTable)
        {
            // VIOLATION cr-dotnet-0014: SqlBulkCopy is SQL Server-specific
            using (var conn = new SqlConnection(_cs))
            {
                conn.Open();
                using (var bulk = new SqlBulkCopy(conn))
                {
                    bulk.DestinationTableName = "dbo.Orders";
                    bulk.WriteToServer(ordersTable);
                }
            }
        }

        public void MergeOrderStatus(string orderId)
        {
            using (var conn = new SqlConnection(_cs))
            {
                conn.Open();
                // VIOLATION cr-dotnet-0014: T-SQL MERGE statement (SQL Server-specific syntax)
                var cmd = new SqlCommand(@"
                    MERGE INTO OrderStatus AS target
                    USING (SELECT @OrderId AS OrderId) AS src ON target.OrderId = src.OrderId
                    WHEN MATCHED THEN UPDATE SET ProcessedDate = GETDATE()
                    WHEN NOT MATCHED THEN INSERT (OrderId, ProcessedDate) VALUES (@OrderId, GETDATE());",
                    conn);
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.ExecuteNonQuery();
            }
        }

        public void EnableChangeTracking()
        {
            using (var conn = new SqlConnection(_cs))
            {
                conn.Open();
                // VIOLATION cr-dotnet-0014: SQL Server-specific Change Tracking DDL
                new SqlCommand("ALTER DATABASE AppDB SET CHANGE_TRACKING = ON (CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON)", conn).ExecuteNonQuery();
            }
        }
    }
}
