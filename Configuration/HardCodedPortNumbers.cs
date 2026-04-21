// =============================================================================
// RULE ID   : cr-dotnet-0017
// RULE NAME : Hard-coded Port Numbers
// CATEGORY  : Configuration
// DESCRIPTION: FIXED - Ports now resolved via AWS Cloud Map service discovery
// =============================================================================
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Amazon.ServiceDiscovery;
using Amazon.ServiceDiscovery.Model;

namespace SyntheticLegacyApp.Configuration
{
    public class HardCodedPortNumbers
    {
        private readonly IAmazonServiceDiscovery _serviceDiscovery;

        public HardCodedPortNumbers(IAmazonServiceDiscovery serviceDiscovery = null)
        {
            _serviceDiscovery = serviceDiscovery ?? new AmazonServiceDiscoveryClient();
        }

        private async Task<(string host, int port)> DiscoverServiceAsync(string serviceName)
        {
            try
            {
                var request = new DiscoverInstancesRequest
                {
                    NamespaceName = Environment.GetEnvironmentVariable("SERVICE_NAMESPACE") ?? "default",
                    ServiceName = serviceName
                };
                var response = await _serviceDiscovery.DiscoverInstancesAsync(request);
                
                if (response.Instances.Count > 0)
                {
                    var instance = response.Instances[0];
                    var host = instance.Attributes.ContainsKey("AWS_INSTANCE_IPV4") 
                        ? instance.Attributes["AWS_INSTANCE_IPV4"] 
                        : instance.Attributes["AWS_INSTANCE_DNS_NAME"];
                    var port = int.Parse(instance.Attributes["AWS_INSTANCE_PORT"]);
                    return (host, port);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service discovery failed for {serviceName}: {ex.Message}");
            }

            // Fallback to environment variables
            var envHost = Environment.GetEnvironmentVariable($"{serviceName.ToUpper()}_HOST") ?? "localhost";
            var envPort = int.Parse(Environment.GetEnvironmentVariable($"{serviceName.ToUpper()}_PORT") ?? "0");
            return (envHost, envPort);
        }

        public async Task<TcpClient> ConnectToDatabaseAsync()
        {
            // FIXED: Port resolved via AWS Cloud Map service discovery
            var (host, port) = await DiscoverServiceAsync("database");
            return new TcpClient(host, port);
        }

        public TcpClient ConnectToDatabase()
        {
            return ConnectToDatabaseAsync().GetAwaiter().GetResult();
        }

        public async Task<IPEndPoint> GetApiEndpointAsync()
        {
            // FIXED: Port resolved via AWS Cloud Map service discovery
            var (host, port) = await DiscoverServiceAsync("internal-api");
            return new IPEndPoint(IPAddress.Parse(host), port);
        }

        public IPEndPoint GetApiEndpoint()
        {
            return GetApiEndpointAsync().GetAwaiter().GetResult();
        }

        public Socket CreateAdminSocket()
        {
            // FIXED: Port from environment variable, allowing dynamic assignment
            var adminPort = int.Parse(Environment.GetEnvironmentVariable("ADMIN_PORT") ?? "0");
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Bind(new IPEndPoint(IPAddress.Any, adminPort));
            return sock;
        }
    }
}
