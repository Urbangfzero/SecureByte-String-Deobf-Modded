using System;
using System.IO;
using System.Security.Cryptography;

namespace SecureByteResourceDecompressor.Decompressor
{
    internal class Decompressor
    {
        #region Decompressor
        public static byte[] Decompress(byte[] data, int keySeed)
        {
            using (var rij = Rijndael.Create())
            {
                rij.Key = SHA256.Create().ComputeHash(BitConverter.GetBytes(keySeed));
                rij.IV = new byte[16];
                rij.Mode = CipherMode.CBC;

                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, rij.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }
        #endregion Decompressor
    }
}