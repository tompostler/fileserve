namespace Unlimitedinf.Fileserve.Config
{
    using Json;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The running program's configuration, in addition to useful methods to work with the configuration.
    /// </summary>
    internal sealed partial class Config
    {
        private readonly List<User> users;
        private readonly List<Json.File> files;
        private readonly Dictionary<Guid, HashSet<Guid>> userfiles;
        private readonly string filename;

        /// <summary>
        /// Default Ctor.
        /// </summary>
        public Config()
        {
            this.users = new List<User>();
            this.files = new List<Json.File>();
            this.userfiles = new Dictionary<Guid, HashSet<Guid>>();
            this.filename = "fileserve.json";
        }

        /// <summary>
        /// Ctor when given a filename.
        /// </summary>
        /// <param name="filename"></param>
        public Config(string filename) : this()
        {
            this.filename = filename;

            if (System.IO.File.Exists(this.filename))
            {
                Overall configFile;

                using (StreamReader sr = new StreamReader(filename))
                using (JsonReader jr = new JsonTextReader(sr))
                {
                    JsonSerializer js = new JsonSerializer();
                    configFile = js.Deserialize<Overall>(jr);
                }

                this.files = configFile.Files;
                this.users = configFile.Users;
                this.userfiles = configFile.UserFiles;
            }
        }
    }
}
