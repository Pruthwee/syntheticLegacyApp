// =============================================================================
// RULE ID   : cr-dotnet-0013
// RULE NAME : SqlConnection Direct Usage
// CATEGORY  : Database
// DESCRIPTION: Application manages SQL Server connections directly without using
//              connection pooling frameworks or cloud-managed connection services.
//              Prevents efficient resource utilization in cloud database services.
// =============================================================================
using System.Data;
using System.Data.SqlClient;

namespace SyntheticLegacyApp.Database
{
    public class SqlConnectionDirect
    {
        private readonly string _cs = "Server=prod-db;Database=AppDB;User Id=appuser;Password=dbpass!;";

        public void InsertOrder(string orderId, decimal amount)
        {
            // VIOLATION cr-dotnet-0013: Raw SqlConnection, not using 'using' - potential leak
            SqlConnection connection = new SqlConnection(_cs);
            connection.Open();
            var cmd = new SqlCommand("INSERT INTO Orders (OrderId, Amount) VALUES (@id, @amt)", connection);
            cmd.Parameters.AddWithValue("@id",  orderId);
            cmd.Parameters.AddWithValue("@amt", amount);
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public DataTable QueryOrders(string status)
        {
            // VIOLATION cr-dotnet-0013: Manual connection management, no retry logic
            var conn = new SqlConnection(_cs);
            conn.Open();
            var adapter = new SqlDataAdapter("SELECT * FROM Orders WHERE Status='" + status + "'", conn);
            var dt = new DataTable();
            adapter.Fill(dt);
            conn.Close();
            return dt;
        }
    }
}
