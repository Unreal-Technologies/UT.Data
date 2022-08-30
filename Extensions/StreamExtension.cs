namespace UT.Data.Extensions
{
    public static class StreamExtension
    {
        public static byte[] Read(this Stream s)
        {
            MemoryStream ms = new();
            byte[] b = new byte[1024];
            int k = s.Read(b, 0, b.Length);
            while (k == b.Length)
            {
                ms.Write(b);
                k = s.Read(b, 0, b.Length);
            }
            ms.Write(b, 0, k);
            return ms.ToArray();
        }
    }
}
