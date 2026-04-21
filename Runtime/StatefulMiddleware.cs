// =============================================================================
// RULE ID   : cr-dotnet-0126
// RULE NAME : Heavy Coupling to Stateful Middleware
// CATEGORY  : Runtime
// DESCRIPTION: Application is tightly integrated with stateful middleware features
//              like IIS application pool sticky sessions, WCF session-based services,
//              that don't translate to cloud-native stateless architectures and
//              horizontal scaling patterns.
// =============================================================================

using System;
using System.ServiceModel;
using System.Web;
using System.Web.SessionState;

namespace SyntheticLegacyApp.Runtime
{
    // VIOLATION cr-dotnet-0126: IRequiresSessionState forces sticky session routing in IIS
    public class StatefulReportHandler : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            // VIOLATION cr-dotnet-0126: Reading accumulated state from in-process session
            var reportData = context.Session["AccumulatedReportData"] as System.Data.DataSet;

            if (reportData == null)
            {
                reportData = new System.Data.DataSet("Report");
                context.Session["AccumulatedReportData"] = reportData; // stored in-proc
            }

            // Add more data to the session-carried DataSet across multiple requests
            var table = reportData.Tables.Add($"Page_{context.Session["PageCount"] ?? 0}");
            table.Columns.Add("Value");
            context.Session["PageCount"] =
                ((int)(context.Session["PageCount"] ?? 0)) + 1;

            context.Response.ContentType = "application/json";
            context.Response.Write($"{{\"pages\": {context.Session["PageCount"]}}}");
        }
    }

    // VIOLATION cr-dotnet-0126: WCF PerSession — session-based service requires sticky routing
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IStatefulOrderService
    {
        [OperationContract(IsInitiating = true)]
        void BeginOrderSession(string customerId);

        [OperationContract]
        void AddLineItem(string productId, int qty);

        [OperationContract(IsTerminating = true)]
        string CommitOrder();
    }

    // VIOLATION cr-dotnet-0126: ServiceBehavior PerSession — WCF maintains instance per client session
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession,
                     ConcurrencyMode     = ConcurrencyMode.Single)]
    public class StatefulOrderService : IStatefulOrderService
    {
        // VIOLATION cr-dotnet-0126: In-service mutable state accumulates across WCF session calls
        private string _customerId;
        private readonly System.Collections.Generic.List<(string ProductId, int Qty)> _lineItems
            = new System.Collections.Generic.List<(string, int)>();

        public void BeginOrderSession(string customerId)
        {
            _customerId = customerId;
            _lineItems.Clear();
            Console.WriteLine($"WCF session started for customer: {customerId}");
        }

        public void AddLineItem(string productId, int qty)
        {
            // VIOLATION cr-dotnet-0126: State held in WCF service instance — lost on any failover
            _lineItems.Add((productId, qty));
            Console.WriteLine($"Line item added: {productId} x{qty}");
        }

        public string CommitOrder()
        {
            string orderId = Guid.NewGuid().ToString("N");
            Console.WriteLine($"Committing {_lineItems.Count} line items for {_customerId}. Order: {orderId}");
            return orderId;
        }
    }

    public class StickySessionLoadBalancerConfig
    {
        // VIOLATION cr-dotnet-0126: Application deliberately configures for sticky sessions
        public void ConfigureStickySessions()
        {
            Console.WriteLine("Configuring ARR affinity cookie for IIS load balancing...");
            // In practice would write to applicationHost.config:
            // <webFarm><server><applicationRequestRouting><affinityCookie ... /></>/>
            // This hard-wires client → server affinity, preventing true horizontal scaling.
        }
    }
}
