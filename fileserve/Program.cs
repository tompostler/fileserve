namespace Unlimitedinf.Fileserve
{
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal static class Program
    {
#if DEBUG
        // Development only block.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "commandline")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.Write(System.String)")]
        static void Main(string[] args)
        {
            Console.Write("VS F5 mode commandline args: ");
            args = Console.ReadLine().Split(' ');
#else
        static void Main(string[] args)
        {
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
                    { "add", config.FileAdd },
                    { "del", config.FileDel },
                    { "edit", () =>
                    {
                        throw new NotImplementedException();
                    } },
                    { "list", config.FileList }
                } },
                { "link", new Dictionary<string, Action>
                {
                    { "add", config.LinkAdd },
                    { "del", () =>
                    {
                        config.LinkDel(Guid.Empty, Guid.Empty);
                    } },
                    { "list", () =>
                    {
                        config.LinkList();
                    } }
                } },
                { "user", new Dictionary<string, Action>
                {
                    { "add", () =>
                    {
                        config.UserAdd(string.Empty, string.Empty);
                    } },
                    { "del", () =>
                    {
                        config.UserDel(Guid.Empty);
                    } },
                    { "edit", () =>
                    {
                        throw new NotImplementedException();
                    } },
                    { "list", () =>
                    {
                        config.UserList();
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
