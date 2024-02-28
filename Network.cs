using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace UT.Data
{
    public class Network
    {
        #region Public Methods
        public static bool IsServerReachable(IPAddress ip, int port)
        {
            TcpClient tcpClient = new();
            try
            {
                tcpClient.Connect(ip, port);
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
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            buffer.Add(ip.Address);
                        }
                    }
                }
            }
            return [.. buffer];
        }
        #endregion //Public Methods
    }
}
