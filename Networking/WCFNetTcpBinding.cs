// =============================================================================
// RULE ID   : cr-dotnet-0019
// RULE NAME : WCF NetTcpBinding
// CATEGORY  : Networking
// DESCRIPTION: Application uses WCF with NetTcpBinding for service communication.
//              Binary TCP protocols do not work well through cloud load balancers
//              and proxy infrastructure, causing service communication failures.
// =============================================================================
using System.ServiceModel;

namespace SyntheticLegacyApp.Networking
{
    [ServiceContract]
    public interface IOrderService
    {
        [OperationContract] string GetOrder(string orderId);
        [OperationContract] void   CreateOrder(string orderId, decimal amount);
    }

    public class WCFNetTcpBinding
    {
        public IOrderService GetOrderServiceClient()
        {
            // VIOLATION cr-dotnet-0019: NetTcpBinding uses binary TCP - blocked by cloud LBs
            var binding  = new NetTcpBinding(SecurityMode.Transport);
            var endpoint = new EndpointAddress("net.tcp://order-svc.corp.internal:8089/OrderService");
            var factory  = new ChannelFactory<IOrderService>(binding, endpoint);
            return factory.CreateChannel();
        }

        public ServiceHost CreateNetTcpHost()
        {
            // VIOLATION cr-dotnet-0019: Self-hosted WCF service on TCP binary port
            var host = new ServiceHost(typeof(OrderServiceImpl));
            host.AddServiceEndpoint(typeof(IOrderService), new NetTcpBinding(),
                "net.tcp://0.0.0.0:8089/OrderService");
            host.Open();
            return host;
        }
    }

    public class OrderServiceImpl : IOrderService
    {
        public string GetOrder(string id)          => "{}";
        public void   CreateOrder(string id, decimal amt) { }
    }
}
