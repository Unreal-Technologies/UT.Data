using System.Security.Cryptography;
using UT.Data.Extensions;

namespace UT.Data.Encryption
{
    public class Aes
    {
        #region Public Methods
        public static byte[] Encrypt(byte[] data, string password)
        {
            return Encrypt(
                data, 
                Key(password), 
                IV(password)
            );
        }

        public static string Encrypt(string text, string password)
        {
            return Convert.ToBase64String(Encrypt(
                text.AsBytes(), 
                Key(password), 
                IV(password)
            ));
        }

        public static byte[]? Decrypt(byte[] value, string password)
        {
            if (value == null)
            {
                return null;
            }
            try
            {
                return Decrypt(
                    value,
                    Key(password),
                    IV(password)
                );
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string? Decrypt(string text, string password)
        {
            if(text == null)
            {
                return null;
            }
            try
            {
                return Decrypt(
                    Convert.FromBase64String(text),
                    Key(password),
                    IV(password)
                ).AsString();
            }
            catch(Exception)
            {
                return null;
            }
        }

        public static byte[] Decrypt(byte[] text, byte[] key, byte[] iv)
        {
            RijndaelManaged aes = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 256,
                BlockSize = 128,
                Key = Aes.FixKeyLength(key, 32),
                IV = iv
            };

            MemoryStream value = new();
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new(text))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    byte[] buffer = new byte[100];
                    int k = 0;
                    while ((k = cs.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        value.Write(buffer, 0, k);
                        if(k < 100)
                        {
                            continue;
                        }
                    }

                    cs.Close();
                }
                ms.Close();
            }

            byte[] data = new byte[value.Length];
            value.Position = 0;
            value.Read(data, 0, data.Length);
            return data;
        }

        public static byte[] Encrypt(byte[] text, byte[] key, byte[] iv)
        {
            RijndaelManaged aes = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 256,
                BlockSize = 128,
                Key = Aes.FixKeyLength(key, 32),
                IV = iv
            };

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            MemoryStream ms = new();
            using (CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(text, 0, text.Length);
                cs.Close();
            }
            byte[] encoded = ms.ToArray();
            ms.Close();


            return encoded;
        }
        #endregion //Public Methods

        #region Private Methods
        private static byte[] IV(string password)
        {
            return password.Md5()[..16].AsBytes();
        }

        private static byte[] Key(string password)
        {
            return Aes.FixKeyLength(password.AsBytes(), 32);
        }

        private static byte[] FixKeyLength(byte[] key, int size)
        {
            if (key.Length < size)
            {
                byte[] padded = new byte[size];
                Buffer.BlockCopy(key, 0, padded, 0, key.Length);
                key = padded;
            }
            else if(key.Length > size)
            {
                byte[] padded = new byte[size];
                Buffer.BlockCopy(key, 0, padded, 0, padded.Length);
                key = padded;
            }

            return key;
        }
        #endregion //Private Methods
    }
}
