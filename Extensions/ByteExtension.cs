using System.Text;

namespace UT.Data.Extensions
{
    public static class ByteExtension
    {
        public static string AsString(this byte[] value)
        {
            return value.AsString(Encoding.UTF8);
        }

        public static string AsString(this byte[] value, Encoding encoding)
        {
            return encoding.GetString(value);
        }
    }
}
