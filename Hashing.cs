using System.Security.Cryptography;
using System.Text;

namespace UT.Data
{
    public class Hashing
    {
        public static Guid Guid(string input)
        {
            byte[] hash = MD5.HashData(Encoding.Default.GetBytes(input));
            return new Guid(hash);
        }
    }
}
