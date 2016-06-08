namespace Unlimitedinf.Fileserve
{
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal static class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            Console.Write("Debug mode commandline args: ");
            args = Console.ReadLine().Split(' ');
#endif

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

            string submodule = args[0];
            string command = args[1];
            string filename = "fileserve.json";

            // If 3, check valid file
            // Can assume user is not trying to be malicious as user is generally me
            if (args.Count == 3)
            {
                try
                {
                    FileInfo fi = new FileInfo(args[2]);
                    filename = fi.FullName;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(Resources.ErrorInvalidFilename, args[2]);
                    Console.WriteLine(ex.Message);
                }
            }

            // Validate CLI
            Dictionary<string, HashSet<string>> validCli = new Dictionary<string, HashSet<string>>
            {
                { "file", new HashSet<string> { "add", "del", "edit", "list" } },
                { "link", new HashSet<string> { "add", "del", "list" } },
                { "user", new HashSet<string> { "add", "del", "edit", "list" } }
            };

            // Check valid submodule
            if (!validCli.Keys.Contains(submodule))
            {
                Console.WriteLine(Resources.ErrorInvalidSubmodule, submodule);
                ShowConfigHelp();
                return;
            }

            // Check valid command
            if (!validCli[submodule].Contains(command))
            {
                Console.WriteLine(Resources.ErrorInvalidCommand, command);
                ShowConfigHelp();
                return;
            }

            // Instance of config
            Config.Config config = new Config.Config(filename);

            // Actual CLI
            Dictionary<string, Dictionary<string, Action>> cli = new Dictionary<string, Dictionary<string, Action>>
            {
                { "file", new Dictionary<string, Action>
                {
                    { "add", () => {
                        var file = config.FileAdd(Tools.GetA.String("Web path: /"), Tools.GetA.String("Absolute path: "));
                        Console.WriteLine(file.Id);
                    } }
                } }
            };

            // Run
            cli[submodule][command]();
            config.WriteToDisk();
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
