using System.Net;
using System.Net.Sockets;

namespace SimpleECommerceBackend.Infrastructure.Contexts;

public sealed class ServerIpAddressContext : IServerIpAddressContext
{
    private const string LoopbackIpAddress = "127.0.0.1";

    public string GetIpAddress()
    {
        try
        {
            var addresses = Dns.GetHostAddresses(Dns.GetHostName());
            var ipAddress = addresses.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork)
                ?? addresses.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetworkV6);

            return ipAddress?.ToString() ?? LoopbackIpAddress;
        }
        catch (SocketException)
        {
            return LoopbackIpAddress;
        }
        catch (InvalidOperationException)
        {
            return LoopbackIpAddress;
        }
    }
}
