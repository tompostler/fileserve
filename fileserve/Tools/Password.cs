namespace Unlimitedinf.Fileserve.Tools
{
    using BCrypt.Net;
    using System;
    /// <summary>
    /// Contains various static methods to handle passwords.
    /// </summary>
    internal static class Password
    {
        /// <summary>
        /// Using BCrypt, hash a user's password. Work factor of 12.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="workFactor"></param>
        /// <returns>A fixed 60-character BCrypt hash.</returns>
        public static string Hash(string password, int workFactor = 12)
        {
            return BCrypt.HashPassword(password, workFactor);
        }

        /// <summary>
        /// Validate a password matches for a hash.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool Validate(string password, string hash)
        {
            return BCrypt.Verify(password, hash);
        }

        /// <summary>
        /// Given a password hash, get the corresponding work factor.
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static int WorkFactor(string hash)
        {
            if (hash == null)
                throw new ArgumentNullException(nameof(hash));
            if (hash.Length != 60)
                throw new ArgumentOutOfRangeException(nameof(hash), "Hash must be 60 characters (bcrypt hash length)");

            // Example hash: $2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy
            // Example hash: $2a$04$6v3HDfAQaih9IwO2O3zo1.Q9mKgXOmAAgYmpa/ejTH4AWiq5oWkFy

            return int.Parse(hash.Substring(4, 2));
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
                
                return Password.Hash(password, Password.WorkFactor(hash) + levels);
            }
            return null;
        }

        /// <summary>
        /// Wrapper for <see cref="GetA.PasswordFromConsole"/>.
        /// </summary>
        /// <returns></returns>
        public static string FromConsole()
        {
            return GetA.PasswordFromConsole();
        }
    }
}
