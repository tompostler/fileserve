namespace Unlimitedinf.Fileserve.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains various static methods to get information from the user.
    /// </summary>
    public static class GetA
    {
        /// <summary>
        /// Get a password from the console as a secure string, printing asteriks instead of the user's password while
        /// they type it in.
        /// </summary>
        /// <remarks>
        /// A la http://stackoverflow.com/a/3404464
        /// </remarks>
        /// <returns></returns>
        public static SecureString PasswordFromConsoleSecurely()
        {
            SecureString pwd = new SecureString();
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.RemoveAt(pwd.Length - 1);
                        Console.Write(Properties.Resources.GetAPasswordBackspace.Replace("\\b", "\b"));
                    }
                }
                else if (!char.IsControl(i.KeyChar))
                {
                    pwd.AppendChar(i.KeyChar);
                    Console.Write(Properties.Resources.GetAPasswordCoverChar);
                }
            }
            return pwd;
        }

        /// <summary>
        /// Get a password from the console, printing asteriks instead of the user's password while they type it in.
        /// </summary>
        /// <remarks>
        /// A la http://stackoverflow.com/a/3404464
        /// </remarks>
        /// <returns></returns>
        public static string PasswordFromConsole()
        {
            StringBuilder pwd = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.Remove(pwd.Length - 1, 1);
                        Console.Write(Properties.Resources.GetAPasswordBackspace.Replace("\\b", "\b"));
                    }
                }
                else if (!char.IsControl(i.KeyChar))
                {
                    pwd.Append(i.KeyChar);
                    Console.Write(Properties.Resources.GetAPasswordCoverChar);
                }
            }
            return pwd.ToString();
        }

        /// <summary>
        /// Get a base64 token.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Token(int length = 64)
        {
            byte[] random = new byte[length * 3 / 4 + 1];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(random);
            }
            return Convert.ToBase64String(random).Substring(0, length);
        }
    }
}
