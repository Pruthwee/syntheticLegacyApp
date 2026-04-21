// =============================================================================
// RULE ID   : cr-dotnet-0017
// RULE NAME : Hard-coded Port Numbers
// CATEGORY  : Configuration
// DESCRIPTION: Application contains hard-coded port numbers for services, databases,
//              or inter-service communication. Prevents dynamic port assignment
//              required by cloud service discovery mechanisms.
// FIXED: Replaced hardcoded ports with environment variable configuration
// =============================================================================
using System;
using System.Net;
using System.Net.Sockets;

namespace SyntheticLegacyApp.Configuration
{
    public class HardCodedPortNumbers
    {
        // FIXED: Use environment variables for port numbers with sensible defaults
        private readonly int SqlServerPort = int.TryParse(
            Environment.GetEnvironmentVariable("SQL_SERVER_PORT"), out int sqlPort) ? sqlPort : 1433;

        private readonly int RedisPort = int.TryParse(
            Environment.GetEnvironmentVariable("REDIS_PORT"), out int redisPort) ? redisPort : 6379;

        private readonly int InternalApiPort = int.TryParse(
            Environment.GetEnvironmentVariable("INTERNAL_API_PORT"), out int apiPort) ? apiPort : 8080;

        private readonly int AdminPort = int.TryParse(
            Environment.GetEnvironmentVariable("ADMIN_PORT"), out int adminPort) ? adminPort : 9090;

        public TcpClient ConnectToDatabase()
        {
            // FIXED: Use environment variable for database server
            string dbServer = Environment.GetEnvironmentVariable("DATABASE_SERVER") ?? "localhost";
            return new TcpClient(dbServer, SqlServerPort);
        }

        public IPEndPoint GetApiEndpoint()
        {
            // FIXED: Use environment variable for API host
            string apiHost = Environment.GetEnvironmentVariable("INTERNAL_API_HOST") ?? "127.0.0.1";
            return new IPEndPoint(IPAddress.Parse(apiHost), InternalApiPort);
        }

        public Socket CreateAdminSocket()
        {
            // FIXED: Use environment-configured admin port
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Bind(new IPEndPoint(IPAddress.Any, AdminPort));
            return sock;
        }
    }
}
