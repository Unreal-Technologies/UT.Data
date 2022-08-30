using System.Net.Sockets;

namespace UT.Data.Extensions
{
    public static class SocketExtension
    {
        public static byte[] Read(this Socket s)
        {
            MemoryStream ms = new();
            byte[] b = new byte[1024];
            int k = s.Receive(b);
            while (k == b.Length)
            {
                ms.Write(b);
                k = s.Receive(b);
            }
            ms.Write(b, 0, k);
            return ms.ToArray();
        }
    }
}
