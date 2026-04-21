// =============================================================================
// RULE ID   : cr-dotnet-0027
// RULE NAME : WCF Service Host
// CATEGORY  : Networking
// DESCRIPTION: Application implements self-hosted WCF services using ServiceHost
//              or custom hosting mechanisms. Self-hosted services have port binding
//              issues and service discovery problems in dynamic cloud environments.
// =============================================================================
using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace SyntheticLegacyApp.Networking
{
    public class WCFServiceHost
    {
        private ServiceHost _invoiceHost;
        private ServiceHost _paymentHost;

        public void StartServices()
        {
            // VIOLATION cr-dotnet-0027: Self-hosted WCF with fixed base address
            _invoiceHost = new ServiceHost(typeof(InvoiceServiceImpl),
                new Uri("http://0.0.0.0:8181/InvoiceService"));
            _invoiceHost.AddServiceEndpoint(typeof(IOrderService), new BasicHttpBinding(), "");

            // VIOLATION cr-dotnet-0027: Enabling WSDL metadata not cloud-native
            _invoiceHost.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
            _invoiceHost.Open();

            // VIOLATION cr-dotnet-0027: Second self-hosted service on another hard-coded port
            _paymentHost = new ServiceHost(typeof(PaymentServiceImpl),
                new Uri("net.tcp://0.0.0.0:8182/PaymentService"));
            _paymentHost.Open();
        }

        public void StopServices()
        {
            _invoiceHost?.Close();
            _paymentHost?.Close();
        }
    }

    public class InvoiceServiceImpl : IOrderService
    {
        public string GetOrder(string id)          => "{}";
        public void   CreateOrder(string id, decimal amt) { }
    }

    public class PaymentServiceImpl : IOrderService
    {
        public string GetOrder(string id)          => "{}";
        public void   CreateOrder(string id, decimal amt) { }
    }
}
