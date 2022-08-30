using System.Reflection;
using UT.Data.Extensions;

namespace UT.Data.IO
{
    public class LocalConfig
    {
        #region Members
        private FileInfo location;
        #endregion //Members

        #region Constructors
        public LocalConfig(string name)
        {
            Assembly assem = Assembly.GetCallingAssembly();
            string? codeBase = assem.GetName().CodeBase;
            if(codeBase == null)
            {
                throw new NotImplementedException("No Codebase");
            }
            DirectoryInfo? dir = (new FileInfo(codeBase.Replace("file:///", ""))).Directory;
            if(dir == null)
            {
                throw new NotImplementedException("No Codebase");
            }

            this.location = new FileInfo(dir.FullName + "\\" + name + ".lc");
        }
        #endregion //Constructors

        #region Properties
        public bool Exists
        {
            get { return this.location.Exists; }
        }
        #endregion //Properties

        #region Public Methods
        public bool Save<T>(T obj)
            where T : class
        {
            byte[] stream = Serializer<T>.Serialize(obj);

            try
            {
                FileStream fs = new(this.location.FullName, FileMode.Create, FileAccess.Write);
                fs.Write(stream, 0, stream.Length);
                fs.Close();
                while(!this.location.Exists)
                {
                    this.location = new FileInfo(this.location.FullName);
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
            FileStream fs = new(this.location.FullName, FileMode.Open, FileAccess.Read);
            byte[] stream = fs.Read();
            fs.Close();

            return Serializer<T>.Deserialize(stream);
        }
        #endregion //Public Methods
    }
}
