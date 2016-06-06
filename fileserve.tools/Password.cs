namespace Unlimitedinf.Fileserve.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains various static methods to handle passwords.
    /// </summary>
    public static class Password
    {
        /// <summary>
        /// Using BCrypt, hash a user's password. Work factor of 12.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="workFactor"></param>
        /// <returns>A fixed 60-character BCrypt hash.</returns>
        public static string Hash(string password, int workFactor = 12)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor);
        }

        /// <summary>
        /// Validate a password matches for a hash.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool Validate(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        /// <summary>
        /// Given a password and its hash, will upgrade the hash's work factor by a certain amount.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="hash"></param>
        /// <param name="levels"></param>
        /// <returns></returns>
        public static string HashUpgrade(string password, string hash, int levels = 1)
        {
            if (Password.Validate(password, hash))
            {
                return Password.Hash(password, levels);
            }
            return null;
        }

        /// <summary>
        /// Wrapper for <see cref="GetA.PasswordFromConsole"/>.
        /// </summary>
        /// <returns></returns>
        public static string GetFromConsole()
        {
            return GetA.PasswordFromConsole();
        }
    }
}
