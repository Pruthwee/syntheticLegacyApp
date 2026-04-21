// =============================================================================
// RULE ID   : cr-dotnet-0017
// RULE NAME : Hard-coded Port Numbers
// CATEGORY  : Configuration
// DESCRIPTION: Application contains hard-coded port numbers for services, databases,
//              or inter-service communication. Prevents dynamic port assignment
//              required by cloud service discovery mechanisms.
// =============================================================================
using System.Net;
using System.Net.Sockets;

namespace SyntheticLegacyApp.Configuration
{
    public class HardCodedPortNumbers
    {
        // VIOLATION cr-dotnet-0017: Port numbers as constants
        private const int SqlServerPort   = 1433;
        private const int RedisPort       = 6379;
        private const int InternalApiPort = 8080;
        private const int AdminPort       = 9090;

        public TcpClient ConnectToDatabase()
        {
            // VIOLATION cr-dotnet-0017: Hard-coded SQL Server port
            return new TcpClient("db-server.corp.internal", SqlServerPort);
        }

        public IPEndPoint GetApiEndpoint()
        {
            // VIOLATION cr-dotnet-0017: Hard-coded API port prevents cloud port remapping
            return new IPEndPoint(IPAddress.Parse("10.0.1.55"), InternalApiPort);
        }

        public Socket CreateAdminSocket()
        {
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Bind(new IPEndPoint(IPAddress.Any, AdminPort)); // VIOLATION cr-dotnet-0017
            return sock;
        }
    }
}
