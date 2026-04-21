// =============================================================================
// RULE ID   : cr-dotnet-0016
// RULE NAME : TransactionScope with DTC
// CATEGORY  : Database
// DESCRIPTION: Application uses System.Transactions.TransactionScope with DTC for
//              distributed transactions. DTC does not work in cloud environments,
//              causing transaction failures and data consistency issues.
// =============================================================================
using System.Transactions;
using System.Data.SqlClient;

namespace SyntheticLegacyApp.Database
{
    public class TransactionScopeWithDTC
    {
        private readonly string _orderCs   = "Server=orders-db;Database=Orders;User Id=app;Password=p;";
        private readonly string _invoiceCs = "Server=invoice-db;Database=Invoices;User Id=app;Password=p;";

        public void ProcessOrderWithDTC(string orderId, decimal amount)
        {
            // VIOLATION cr-dotnet-0016: TransactionScope spanning two SqlConnections escalates to DTC
            using (var scope = new TransactionScope())
            {
                using (var orderConn = new SqlConnection(_orderCs))
                {
                    orderConn.Open();
                    new SqlCommand("UPDATE Orders SET Status='Processed' WHERE Id='" + orderId + "'",
                        orderConn).ExecuteNonQuery();
                }

                // VIOLATION cr-dotnet-0016: Second connection triggers DTC promotion
                using (var invoiceConn = new SqlConnection(_invoiceCs))
                {
                    invoiceConn.Open();
                    new SqlCommand("INSERT INTO Invoices (OrderId, Amount) VALUES ('" + orderId + "'," + amount + ")",
                        invoiceConn).ExecuteNonQuery();
                }

                scope.Complete();
            }
        }

        public void NestedDTCScope()
        {
            // VIOLATION cr-dotnet-0016: Nested TransactionScope with RequiresNew
            using (var outer = new TransactionScope())
            {
                using (var inner = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    inner.Complete();
                }
                outer.Complete();
            }
        }
    }
}
