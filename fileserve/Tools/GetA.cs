namespace Unlimitedinf.Fileserve.Tools
{
    using System;
    using System.Security;
    using System.Text;

    /// <summary>
    /// Contains various static methods to get information from the user via the console.
    /// </summary>
    internal static class GetA
    {
        /// <summary>
        /// Get a password from the console as a secure string, printing asteriks instead of the user's password while
        /// they type it in.
        /// </summary>
        /// <remarks>
        /// A la http://stackoverflow.com/a/3404464
        /// </remarks>
        /// <returns></returns>
        public static SecureString PasswordSecurely()
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
        public static string Password()
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
        /// Get a string from the console.
        /// </summary>
        /// <param name="prompt">Optional prompt to put before reading the string.</param>
        /// <returns></returns>
        public static string String(string prompt = null)
        {
            if (!string.IsNullOrEmpty(prompt))
                Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}
