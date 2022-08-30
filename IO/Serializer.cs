using System.Text;
using System.Text.Json;

namespace UT.Data.IO
{
    public class Serializer<T>
        where T : class
    {
        #region Public Methods
        public static byte[] Serialize(T obj)
        {
            string json = JsonSerializer.Serialize(obj);
            byte[] stream = ASCIIEncoding.UTF8.GetBytes(json);

            return stream;
        }

        public static T? Deserialize(byte[] stream)
        {
            string json = ASCIIEncoding.UTF8.GetString(stream);
            return JsonSerializer.Deserialize<T>(json);
        }
        #endregion //Public Methods
    }
}
