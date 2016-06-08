namespace Unlimitedinf.Fileserve
{
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal sealed class Program
    {
        static void Main(string[] args)
        {
            // Handle the CLI
            int len = args.Length;
            if (len > 0)
            {
                if (len == 1)
                {
                    if (args[0] == "help")
                        ShowHelp();
                }
                else if (args[0] == "help")
                {
                    if (args[1] == "config")
                        ShowConfigHelp();
                }
                else if (args[0] == "config")
                {
                    List<string> cargs = args.ToList();
                    cargs.RemoveAt(0);
                    RunConfig(cargs);
                }
                else
                {
                    Console.WriteLine(Resources.ErrorIdk);
                }
            }
        }

        /// <summary>
        /// Runs the configuration module.
        /// </summary>
        /// <param name="args">The starting args, minus the first one.</param>
        private static void RunConfig(List<string> args)
        {
            // Check right nubmer of args
            if (args.Count != 2 && args.Count != 3)
            {
                Console.WriteLine(Resources.ErrorIncorrectArgCount, "config", args.Count);
                ShowConfigHelp();
                return;
            }

            // If 3, check valid file
            // Can assume user is not trying to be malicious as user is generally me
            if (args.Count == 3)
            {
                try
                {
                    FileInfo fi = new FileInfo(args[2]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(Resources.ErrorInvalidFilename, args[2]);
                    Console.WriteLine(ex.Message);
                }
            }

            // Valid CLI
            Dictionary<string, HashSet<string>> validCli = new Dictionary<string, HashSet<string>>
            {
                { "file", new HashSet<string> { "add", "del", "edit", "list" } },
                { "link", new HashSet<string> { "add", "del", "list" } },
                { "user", new HashSet<string> { "add", "del", "edit", "list" } }
            };

            // Check valid submodule
            if (!validCli.Keys.Contains(args[0]))
            {
                Console.WriteLine(Resources.ErrorInvalidSubmodule, args[0]);
                ShowConfigHelp();
                return;
            }

            // Check valid command
            if (!validCli[args[0]].Contains(args[1]))
            {
                Console.WriteLine(Resources.ErrorInvalidCommand, args[1]);
                ShowConfigHelp();
                return;
            }

            Config.Config config;
            if (args.Count == 2)
                config = new Config.Config();
            else
                config = new Config.Config(args[2]);
        }

        /// <summary>
        /// Help text for the overall program usage.
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine(Resources.ProgramHelp);
        }

        /// <summary>
        /// Help text for the config module.
        /// </summary>
        private static void ShowConfigHelp()
        {
            Console.WriteLine(Resources.ProgramHelpConfig);
        }
    }
}
