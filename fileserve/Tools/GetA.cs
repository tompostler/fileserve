﻿namespace Unlimitedinf.Fileserve.Tools
{
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security;
    using System.Text;
    using Unlimitedinf.Tools;

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
        public static string Password(string prompt = null)
        {
            if (!string.IsNullOrEmpty(prompt))
                Console.Write(prompt);

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

        /// <summary>
        /// Get an Id from the console. Returns blank Id on failure.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="retryCount"></param>
        /// <returns></returns>
        public static Id Id(string prompt = null, int retryCount = 3)
        {
            while (retryCount-- > 0)
            {
                string text = GetA.String(prompt);
                if (string.IsNullOrEmpty(text))
                    break;
                try
                {
                    Id g = new Id(text);
                    return g;
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine(Resources.ErrorInvalidId, text);
                }
            }
            return Unlimitedinf.Tools.Id.Empty;
        }

        /// <summary>
        /// Get a valid Url path from the console.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="retryCount"></param>
        /// <returns></returns>
        public static string UrlPath(string prompt = null, int retryCount = 3)
        {
            while (retryCount-- > 0)
            {
                string text = GetA.String(prompt);
                if (string.IsNullOrEmpty(text))
                    break;

                // Not allowed
                HashSet<string> invalidFilenames = new HashSet<string>()
                {
                    "favicon.ico",
                    "logout",
                    "teapot"
                };
                if (invalidFilenames.Contains(text))
                    break;

                Uri uri;
                if (!Uri.TryCreate(text, UriKind.Relative, out uri))
                    Console.WriteLine(Resources.ErrorInvalidWebPath, text);
                else
                    return Uri.EscapeUriString(text);
            }
            return null;
        }

        /// <summary>
        /// Get an absolute path to a file from the console.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="retryCount"></param>
        /// <param name="existenceCheck">As part of checking for a successful path, check if the file exists.</param>
        /// <returns></returns>
        public static string FileAbsPath(string prompt = null, int retryCount = 3, bool existenceCheck = true)
        {
            while (retryCount-- > 0)
            {
                string text = GetA.String(prompt);
                if (string.IsNullOrEmpty(text))
                    break;

                try
                {
                    string path = Path.GetFullPath(text);
                    if (!existenceCheck || File.Exists(path))
                        return path;
                    else
                        Console.WriteLine(Resources.ErrorFileNotFound, path);
                }
                catch (Exception ex) when (ex is ArgumentException)
                {
                    Console.WriteLine(Resources.ErrorInvalidFilename, text);
                }
            }
            return null;
        }

        /// <summary>
        /// Get an unsigned integer from the console.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="retryCount"></param>
        /// <returns></returns>
        public static uint? Uint(string prompt = null, int retryCount = 3)
        {
            while (retryCount-- > 0)
            {
                string text = GetA.String(prompt);
                if (string.IsNullOrEmpty(text))
                    break;

                uint ui;
                if (!uint.TryParse(text, out ui))
                    Console.WriteLine(Resources.ErrorInvalidUint, text);
                else
                    return ui;
            }
            return null;
        }

        /// <summary>
        /// Get a yes/no confirmation from the user. Appends ' (y/N): ' to the prompt.
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns></returns>
        public static bool YesNo(string prompt = null)
        {
            string text = GetA.String($"{prompt}{(prompt == null ? "" : " ")}{Resources.GetAYesNoPrompt}");
            return text.ToUpperInvariant() == Resources.GetAYesNoY;
        }
    }
}
