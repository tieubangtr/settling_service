using System.Security.Cryptography;
using System.Text;

namespace SettlingEngine.Utils
{
    public static class EncryptData
    {
        public static string EncryptAES(string plainText, string Key, string IV)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(plainText))
                {
                    return string.Empty;
                }

                using Aes aesAlg = Aes.Create();
                aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                aesAlg.IV = Encoding.UTF8.GetBytes(IV);
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                byte[] encryptedBytes;

                using (MemoryStream msEncrypt = new())
                {
                    using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using StreamWriter swEncrypt = new(csEncrypt);
                        swEncrypt.Write(plainText);
                    }
                    encryptedBytes = msEncrypt.ToArray();
                }

                return Convert.ToBase64String(encryptedBytes);
            }
            catch (Exception ex)
            {
                Logger.log.Error("Error exception when EncryptStringAES . Exception : {0}", ex);
                return "";
            }
        }

        public static string DecryptAES(string CipherText, string Key, string IV)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CipherText))
                {
                    return string.Empty;
                }

                byte[] CipherTextByte = Convert.FromBase64String(CipherText);
                byte[] KeyByte = Encoding.UTF8.GetBytes(Key);
                byte[] IVByte = Encoding.UTF8.GetBytes(IV);
                string plaintext = null;
                // Create AesManaged
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Padding = PaddingMode.PKCS7;

                    aesAlg.Key = KeyByte;
                    aesAlg.IV = IVByte;

                    aesAlg.Mode = CipherMode.CBC;
                    // Create a decryptor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    // Create the streams used for decryption.
                    using MemoryStream msDecrypt = new(CipherTextByte);
                    using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                    using StreamReader srDecrypt = new(csDecrypt);
                    // Read the decrypted bytes from the decrypting stream
                    // and place them in a string.
                    plaintext = srDecrypt.ReadToEnd();
                }
                return plaintext;
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Error exception when DecryptAES with CipherText={CipherText},Key={Key},IV={IV} . Exception : {0}", ex);
                throw;
            }
        }
    }
}