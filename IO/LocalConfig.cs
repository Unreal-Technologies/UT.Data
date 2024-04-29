using System.Reflection;
using UT.Data.Encryption;
using UT.Data.Extensions;

namespace UT.Data.IO
{
    public class LocalConfig
    {
        #region Members
        private FileInfo location;
        #endregion //Members

        #region Constants
        private const string Key = "0x4571A4D8";
        #endregion //Constants

        #region Constructors
        public LocalConfig(string name)
        {
            Assembly assem = Assembly.GetCallingAssembly();
            DirectoryInfo? dir = (new FileInfo(assem.Location)).Directory ?? throw new NotImplementedException("No Codebase");
            location = new FileInfo(dir.FullName + "\\" + name + ".lc");
        }
        #endregion //Constructors

        #region Properties
        public string Path
        {
            get { return location.Name; }
        }

        public bool Exists
        {
            get { return location.Exists; }
        }
        #endregion //Properties

        #region Public Methods
        public bool Save<T>(T obj)
            where T : class
        {
            byte[] stream = Aes.Encrypt(Serializer<T>.Serialize(obj), LocalConfig.Key);

            try
            {
                FileStream fs = new(location.FullName, FileMode.Create, FileAccess.Write);
                fs.Write(stream, 0, stream.Length);
                fs.Close();
                while(!location.Exists)
                {
                    location = new FileInfo(location.FullName);
                    Thread.Sleep(5);
                }
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public T? Load<T>()
            where T : class
        {
            FileStream fs = new(location.FullName, FileMode.Open, FileAccess.Read);
            byte[] stream = fs.Read();
            fs.Close();

            return Serializer<T>.Deserialize(Aes.Decrypt(stream, LocalConfig.Key) ?? []);
        }
        #endregion //Public Methods
    }
}
