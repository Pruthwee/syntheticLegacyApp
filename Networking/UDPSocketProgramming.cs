// =============================================================================
// RULE ID   : cr-dotnet-0020
// RULE NAME : UDP Socket Programming
// CATEGORY  : Networking
// DESCRIPTION: Application implements UDP socket programming for network
//              communication. UDP does not work reliably in cloud networking
//              due to load balancing, NAT, and firewall configurations.
// =============================================================================
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SyntheticLegacyApp.Networking
{
    public class UDPSocketProgramming
    {
        private readonly UdpClient _udpClient;

        public UDPSocketProgramming()
        {
            // VIOLATION cr-dotnet-0020: UDP listener - not cloud load-balancer compatible
            _udpClient = new UdpClient(5000);
        }

        public void BroadcastInventoryUpdate(string productId, int quantity)
        {
            // VIOLATION cr-dotnet-0020: UDP broadcast - will not traverse cloud NAT
            using (var sender = new UdpClient())
            {
                sender.EnableBroadcast = true;
                byte[] data = Encoding.UTF8.GetBytes(productId + ":" + quantity);
                sender.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, 5001));
            }
        }

        public void SendStatusPing(string hostName)
        {
            // VIOLATION cr-dotnet-0020: UDP datagram - unreliable in cloud environments
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var ep   = new IPEndPoint(Dns.GetHostAddresses(hostName)[0], 7);
            sock.SendTo(Encoding.UTF8.GetBytes("PING"), ep);
        }
    }
}
