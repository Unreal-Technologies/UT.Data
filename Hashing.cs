using System.Security.Cryptography;
using System.Text;

namespace UT.Data
{
    public class Hashing
    {
        public static Guid Guid(string input)
        {
            using MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
            return new Guid(hash);
        }
    }
}
