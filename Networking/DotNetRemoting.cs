// =============================================================================
// RULE ID   : cr-dotnet-0018
// RULE NAME : .NET Remoting Usage
// CATEGORY  : Networking
// DESCRIPTION: Application uses .NET Remoting for distributed communication.
//              .NET Remoting has poor compatibility with cloud load balancers and
//              modern cloud networking infrastructure.
// =============================================================================
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace SyntheticLegacyApp.Networking
{
    // VIOLATION cr-dotnet-0018: MarshalByRefObject marks class for .NET Remoting
    public class RemoteOrderService : MarshalByRefObject
    {
        public string GetOrderStatus(string orderId) => "Pending";
        public void   ProcessOrder(string orderId)   { }
    }

    public class DotNetRemoting
    {
        public void RegisterRemoteService()
        {
            // VIOLATION cr-dotnet-0018: Registering .NET Remoting TCP channel
            var channel = new TcpChannel(9000);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(RemoteOrderService), "OrderService", WellKnownObjectMode.Singleton);
        }

        public RemoteOrderService GetRemoteProxy()
        {
            // VIOLATION cr-dotnet-0018: Activating remote object via TCP
            return (RemoteOrderService)Activator.GetObject(
                typeof(RemoteOrderService),
                "tcp://order-service.corp.internal:9000/OrderService");
        }
    }
}
