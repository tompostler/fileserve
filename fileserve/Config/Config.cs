namespace Unlimitedinf.Fileserve.Config
{
    using Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The running program's configuration, in addition to useful methods to work with the configuration.
    /// </summary>
    internal sealed partial class Config
    {
        private readonly List<User> users;
        private readonly List<File> files;
        private readonly Dictionary<Guid, HashSet<Guid>> userfiles;

        /// <summary>
        /// Default Ctor.
        /// </summary>
        public Config()
        {
            this.users = new List<User>();
            this.files = new List<File>();
            this.userfiles = new Dictionary<Guid, HashSet<Guid>>();
        }

        /// <summary>
        /// Ctor to use when reading the configuration from a file.
        /// </summary>
        /// <param name="fileConfig"></param>
        public Config(Overall fileConfig)
        {
            this.users = fileConfig.Users;
            this.files = fileConfig.Files;
            this.userfiles = fileConfig.UserFiles;
        }
    }
}
