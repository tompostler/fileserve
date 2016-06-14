namespace Unlimitedinf.Fileserve.Tools
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Contains various static methods to generate information from different sources.
    /// </summary>
    internal static class GenA
    {
        /// <summary>
        /// Get a base64 token.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Token(int length = 64)
        {
            byte[] random = GenA.ByteArray(length * 3 / 4 + 1);
            return Convert.ToBase64String(random).Substring(0, length);
        }

        /// <summary>
        /// Get a hexadecimal token.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string HexToken(int length = 64)
        {
            byte[] random = GenA.ByteArray(length / 2 + 1);
            StringBuilder hex = new StringBuilder(length);
            foreach (byte b in random)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString().Substring(0, length);
        }

        /// <summary>
        /// Generate a byte array filled with random bytes.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] ByteArray(int length = 4)
        {
            byte[] random = new byte[length];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(random);
            }
            return random;
        }
    }
}
