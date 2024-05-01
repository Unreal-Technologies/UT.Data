using System.Security.Cryptography;
using UT.Data.Extensions;

namespace UT.Data.Encryption
{
    public static class Aes
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

        public static string? Encrypt(string? text, string password)
        {
            if(text == null)
            {
                return null;
            }
            return Convert.ToBase64String(Encrypt(
                text.AsBytes(), 
                Key(password), 
                IV(password)
            ));
        }

        public static byte[] Encrypt(byte[] text, byte[] key, byte[] iv)
        {
            var aes = System.Security.Cryptography.Aes.Create();
            aes.IV = iv;
            aes.Padding = PaddingMode.Zeros;
            aes.Mode = CipherMode.CBC;
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Key = Aes.FixKeyLength(key, 32);

            return aes.EncryptCbc(text, iv, PaddingMode.PKCS7);
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

        public static string? Decrypt(string? text, string password)
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
            var aes = System.Security.Cryptography.Aes.Create();
            aes.IV = iv;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Key = Aes.FixKeyLength(key, 32);

            return aes.DecryptCbc(text, iv, PaddingMode.PKCS7);
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
