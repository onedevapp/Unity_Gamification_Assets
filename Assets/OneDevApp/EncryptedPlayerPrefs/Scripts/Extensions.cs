using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace OneDevApp
{
    public static class Extensions
    {
        #region Public variables

        //To Encrypt byte values
        public static byte[] GetEncrypted(this byte[] value)
        {
            using (AesManaged aesManaged = new AesManaged())
            {
                using (MemoryStream memoryStream = new System.IO.MemoryStream())
                {
                    // We're using the PBKDF2 standard for password-based key generation
                    Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes("P@ssw0rd", Encoding.UTF8.GetBytes("S@1tS@1t"));

                    // Setting our parameters
                    aesManaged.BlockSize = aesManaged.LegalBlockSizes[0].MaxSize;
                    aesManaged.KeySize = aesManaged.LegalKeySizes[0].MaxSize;

                    aesManaged.Key = rfc.GetBytes(aesManaged.KeySize / 8);
                    aesManaged.IV = rfc.GetBytes(aesManaged.BlockSize / 8);

                    using (CryptoStream cryptoStream = new CryptoStream(
                        memoryStream,
                        aesManaged.CreateEncryptor(aesManaged.Key, aesManaged.IV),
                        CryptoStreamMode.Write))
                    {
                        using (BinaryWriter binaryWriter = new BinaryWriter(cryptoStream))
                        {
                            binaryWriter.Write(value);
                        }
                        return memoryStream.ToArray();
                    }
                }
            }
        }

        //To Decrypt byte values
        public static byte[] GetDecrypted(this byte[] value)
        {
            using (AesManaged aesManaged = new AesManaged())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // We're using the PBKDF2 standard for password-based key generation
                    Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes("P@ssw0rd", Encoding.UTF8.GetBytes("S@1tS@1t"));

                    // Setting our parameters
                    aesManaged.BlockSize = aesManaged.LegalBlockSizes[0].MaxSize;
                    aesManaged.KeySize = aesManaged.LegalKeySizes[0].MaxSize;

                    aesManaged.Key = rfc.GetBytes(aesManaged.KeySize / 8);
                    aesManaged.IV = rfc.GetBytes(aesManaged.BlockSize / 8);
                    
                    using (CryptoStream cryptoStream = new CryptoStream(
                        memoryStream,
                        aesManaged.CreateDecryptor(aesManaged.Key, aesManaged.IV),
                        CryptoStreamMode.Write))
                    {
                        using (BinaryWriter binaryWriter = new BinaryWriter(cryptoStream))
                        {
                            binaryWriter.Write(value);
                        }
                        return memoryStream.ToArray();
                    }
                }
            }
        }

        //To Encrypt string values
        public static string GetEncrypted(this string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            var encrypted = bytes.GetEncrypted();
            return Convert.ToBase64String(encrypted);
        }

        //To decrypt string values
        public static string GetDecrypted(this string value)
        {
            try
            {
                var bytes = Convert.FromBase64String(value);
                var decrypted = bytes.GetDecrypted();
                return Encoding.UTF8.GetString(decrypted);
            }
            catch
            {
                // return original value if cannot be decrypted
                return value;
            }
        }
        #endregion
    }
}

