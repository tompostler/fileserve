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
                List<string> cargs = args.ToList();
                cargs.RemoveAt(0);
                if (len == 1)
                {
                    if (args[0] == "help")
                        ShowHelp();
                    if (args[0] == "serve")
                        RunServe();
                }
                else if (args[0] == "help")
                {
                    if (args[1] == "config")
                        ShowConfigHelp();
                    if (args[1] == "serve")
                        ShowServeHelp();
                }
                else if (args[0] == "config")
                {
                    RunConfig(cargs);
                }
                else if (args[0] == "serve")
                {
                    RunServe(cargs);
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

            // Instance of config
            Config.Config config = new Config.Config(filename);

            // CLI
            Dictionary<string, Dictionary<string, Action>> cli = new Dictionary<string, Dictionary<string, Action>>
            {
                { "file", new Dictionary<string, Action>
                {
                    { "add", config.FileAdd },
                    { "del", config.FileDel },
                    { "edit", config.FileEdit },
                    { "list", config.FileList }
                } },
                { "link", new Dictionary<string, Action>
                {
                    { "add", config.LinkAdd },
                    { "del", config.LinkDel },
                    { "list", config.LinkList }
                } },
                { "user", new Dictionary<string, Action>
                {
                    { "add", config.UserAdd },
                    { "del", config.UserDel },
                    { "edit", config.UserEdit },
                    { "list", config.UserList }
                } }
            };

            // Check valid submodule
            if (!cli.Keys.Contains(submodule))
            {
                Console.WriteLine(Resources.ErrorInvalidSubmodule, submodule);
                ShowConfigHelp();
                return;
            }

            // Check valid command
            if (!cli[submodule].Keys.Contains(command))
            {
                Console.WriteLine(Resources.ErrorInvalidCommand, command);
                ShowConfigHelp();
                return;
            }

            // Run
            cli[submodule][command]();
            config.WriteToDisk();
        }

        /// <summary>
        /// Runs the serve module.
        /// </summary>
        /// <param name="args">The starting args, minus the first one.</param>
        private static void RunServe(List<string> args = null)
        {
            // Check args
            if (args != null && args.Count == 0)
            {
                Console.WriteLine(Resources.ErrorFileNotFound, "<empty>");
                return;
            }
            string filename = args == null ? "fileserve.json" : args[0];
            // Check valid config file
            if (!File.Exists(filename))
            {
                Console.WriteLine(Resources.ErrorFileNotFound, filename);
                return;
            }

            Config.Config config = new Config.Config(filename);

            // Check valid config files
            string invalid = config.ValidateFiles();
            if (!String.IsNullOrEmpty(invalid))
            {
                Console.WriteLine(Resources.ErrorFileNotFound, invalid);
                return;
            }

            int port = 80;
            if (args != null && args.Count == 2)
            {
                if (!int.TryParse(args[1], out port))
                {
                    Console.WriteLine(Resources.ErrorInvalidPort, port);
                    return;
                }
            }

            // Run the server
            using (Server.FileServe fileserve = new Server.FileServe(config, port))
            {
                fileserve.Start();
                Console.WriteLine(Resources.ProgramQToQuitRToRestart);

                string response = "";
                do
                {
                    response = Console.ReadLine();
                    if (response == "r")
                    {
                        // Assume config file name has not changed and the file still exists
                        config = new Config.Config(filename);
                        // Check valid config files
                        invalid = config.ValidateFiles();
                        if (!String.IsNullOrEmpty(invalid))
                        {
                            Console.WriteLine(Resources.ErrorFileNotFound, invalid);
                            continue;
                        }
                        fileserve.UpdateConfig(config);
                    }
                }
                while (response != "q");

                fileserve.Stop();
            }
        }

        /// <summary>
        /// Help text for the overall program usage.
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine(Resources.ProgramHelp, $"Fileserve by UnlimitedInf v{typeof(Program).Assembly.GetName().Version}");
        }

        /// <summary>
        /// Help text for the config module.
        /// </summary>
        private static void ShowConfigHelp()
        {
            Console.WriteLine(Resources.ProgramHelpConfig);
        }

        /// <summary>
        /// Help text for the serve module.
        /// </summary>
        private static void ShowServeHelp()
        {
            Console.WriteLine(Resources.ProgramHelpServe);
        }
    }
}
