using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Linq;

namespace UT.Data
{
    public static class Network
    {
        #region Public Methods
        public static bool IsServerReachable(IPAddress ip, int port)
        {
            TcpClient tcpClient = new()
            {
                SendTimeout = 1000,
                ReceiveTimeout = 1000,
            };
            try
            {
                if(!tcpClient.ConnectAsync(ip, port).Wait(1000))
                {
                    return false;
                }
                tcpClient.Close();

                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public static IPAddress[] LocalIPv4(NetworkInterfaceType nit)
        {
            List<IPAddress> buffer = [];
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == nit && item.OperationalStatus == OperationalStatus.Up)
                {
                    buffer.AddRange(from UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses
                                    where ip.Address.AddressFamily == AddressFamily.InterNetwork
                                    select ip.Address);
                }
            }
            return [.. buffer];
        }
        #endregion //Public Methods
    }
}
