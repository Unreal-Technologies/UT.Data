namespace UT.Data.Extensions
{
    public static class FileInfoExtension
    {
        public static string ShortName(this FileInfo fi)
        {
            return fi.Name.Remove(fi.Name.Length - fi.Extension.Length, fi.Extension.Length);
        }
    }
}
