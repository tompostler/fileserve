namespace Unlimitedinf.Fileserve.Config
{
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
        private readonly List<Json.File> files;
        private readonly List<Json.User> users;
        private readonly Dictionary<Guid, HashSet<Guid>> links;
        private readonly string filename;
        
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="filename"></param>
        public Config(string filename)
        {
            this.filename = filename;

            // If the file exists, read the configuration from it.
            // Else create a new configuration.
            if (System.IO.File.Exists(this.filename))
            {
                Json.Overall configFile;

                using (StreamReader sr = new StreamReader(filename))
                using (JsonReader jr = new JsonTextReader(sr))
                {
                    JsonSerializer js = new JsonSerializer();
                    configFile = js.Deserialize<Json.Overall>(jr);
                }

                this.files = configFile.Files;
                this.users = configFile.Users;
                this.links = configFile.Links;
            }
            else
            {
                this.users = new List<Json.User>();
                this.files = new List<Json.File>();
                this.links = new Dictionary<Guid, HashSet<Guid>>();
            }
        }

        /// <summary>
        /// Write the configuration to disk.
        /// </summary>
        /// <param name="prettify"></param>
        public void WriteToDisk(bool prettify = true)
        {
            this.Clean();

            Json.Overall configFile = new Json.Overall()
            {
                Files = this.files,
                Users = this.users,
                Links = this.links
            };

            using (StreamWriter sw = new StreamWriter(filename))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                JsonSerializer js = new JsonSerializer();
                if (prettify)
                    js.Formatting = Formatting.Indented;
                js.Serialize(jw, configFile);
            }
        }

        /// <summary>
        /// Cleans the links of empty sets.
        /// </summary>
        private void Clean()
        {
            var toRemove = this.links.Where((link) => link.Value.Count == 0);
            foreach (var link in toRemove)
                this.links.Remove(link.Key);
        }
    }
}
