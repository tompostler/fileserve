namespace Unlimitedinf.Fileserve
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class Program
    {
        private enum Commands
        {
            ConfigFileAdd,
            ConfigFileDel,
            ConfigFileEdit,
            ConfigFileView,
            ConfigLinkAdd,
            ConfigLinkDel,
            ConfigLinkView,
            ConfigUserAdd,
            ConfigUserDel,
            ConfigUserEdit,
            ConfigUserView,
            Help
        };

        private static Dictionary<string, Dictionary<string, Dictionary<string, Action>>> cli = new Dictionary<string, Dictionary<string, Dictionary<string, Action>>>
        {
            { "config", new Dictionary<string, Dictionary<string, Action>>
            {
                { "file", new Dictionary<string, Action>
                {
                    { "add", () => Run(Commands.ConfigFileAdd) },
                    { "del", () => Run(Commands.ConfigFileDel) },
                    { "edit", () => Run(Commands.ConfigFileEdit) },
                    { "view", () => Run(Commands.ConfigFileView) }
                } },
                { "link", new Dictionary<string, Action>
                {
                    { "add", () => Run(Commands.ConfigLinkAdd) },
                    { "del", () => Run(Commands.ConfigLinkDel) },
                    { "view", () => Run(Commands.ConfigLinkView) }
                } },
                { "user", new Dictionary<string, Action>
                {
                    { "add", () => Run(Commands.ConfigUserAdd) },
                    { "del", () => Run(Commands.ConfigUserDel) },
                    { "edit", () => Run(Commands.ConfigUserEdit) },
                    { "view", () => Run(Commands.ConfigUserView) }
                } }
            } }
        };

        static void Main(string[] args)
        {
            // Asking for help somewhere, so just provide whole list
            if (args.Contains("help") || args.Length == 0)
            {
                Run();
            }
        }

        private static void Run(Commands command = Commands.Help)
        {
            const string exeName = "fileserve";

            switch (command)
            {
                case Commands.Help:
                    Console.WriteLine(exeName);
                    foreach (string module in cli.Keys.OrderBy((key) => key))
                    {
                        Console.WriteLine("  " + module);
                        foreach (string submodule in cli[module].Keys.OrderBy((key) => key))
                        {
                            Console.WriteLine("    " + submodule);
                            foreach (string action in cli[module][submodule].Keys.OrderBy((key) => key))
                            {
                                Console.WriteLine("      " + action);
                            }
                        }
                    }
                    break;
            }
        }
    }
}
