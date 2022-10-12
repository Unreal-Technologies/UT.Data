using UT.Data.IO;

namespace UT.Data.Modlet
{
    public class Packet<TDescription, TData>
    {
        #region Public Methods
        public static byte[] Encode(TDescription description, TData data)
        {
            Packet<TDescription, TData> p = new(description, data);
            return Serializer<Packet<TDescription, TData>>.Serialize(p);
        }

        public static Packet<TDescription, TData>? Decode(byte[]? data)
        {
            if(data == null)
            {
                return null;
            }
            return Serializer<Packet<TDescription, TData>>.Deserialize(data);
        }
        #endregion //public Methods

        #region Properties
        public TDescription? Description { get; set; }
        public TData? Data { get; set; }
        #endregion //Properties

        #region Constructors
        public Packet() { }

        internal Packet(TDescription description, TData data)
        {
            this.Description = description;
            this.Data = data;
        }
        #endregion //Constructors
    }
}
