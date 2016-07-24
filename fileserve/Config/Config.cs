namespace Unlimitedinf.Fileserve.Config
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Tools;

    /// <summary>
    /// The running program's configuration, in addition to useful methods to work with the configuration.
    /// </summary>
    internal sealed partial class Config
    {
        private readonly List<Json.File> files;
        private readonly List<Json.User> users;
        private readonly Dictionary<Id, HashSet<Id>> links;
        private readonly string filename;

        private HashSet<Id> fileIds { get; }
        private HashSet<Id> userIds { get; }
        private HashSet<Id> allIds { get; }
        private HashSet<string> usernames { get; }
        private Dictionary<Id, Json.File> filesById { get; }
        private Dictionary<Id, Json.User> usersById { get; }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="filename"></param>
        public Config(string filename)
        {
            this.filename = filename;

            // If the file exists, read the configuration from it.
            // Else create a new configuration.
            if (File.Exists(this.filename))
            {
                Json.Overall configFile;

                StreamReader sr = new StreamReader(filename);
                using (JsonReader jr = new JsonTextReader(sr))
                {
                    JsonSerializer js = new JsonSerializer();
                    configFile = js.Deserialize<Json.Overall>(jr);
                }

                this.files = configFile.Files;
                this.users = configFile.Users;
                this.links = configFile.Links;

                // Convenience collection initialization
                this.fileIds = new HashSet<Id>(this.files.ConvertAll((file) => file.Id));
                this.userIds = new HashSet<Id>(this.users.ConvertAll((user) => user.Id));
                this.allIds = new HashSet<Id>(this.fileIds.Union(this.userIds));
                this.usernames = new HashSet<string>(this.users.ConvertAll((user) => user.Username));
                this.filesById = new Dictionary<Id, Json.File>(this.files.ToDictionary((file) => file.Id));
                this.usersById = new Dictionary<Id, Json.User>(this.users.ToDictionary((user) => user.Id));
            }
            else
            {
                this.users = new List<Json.User>();
                this.files = new List<Json.File>();
                this.links = new Dictionary<Id, HashSet<Id>>();
            }
        }

        /// <summary>
        /// Write the configuration to disk.
        /// </summary>
        public void WriteToDisk()
        {
            this.Clean();

            Json.Overall configFile = new Json.Overall()
            {
                Files = this.files,
                Users = this.users,
                Links = this.links
            };

            StreamWriter sw = new StreamWriter(filename);
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                JsonSerializer js = new JsonSerializer();
#if DEBUG
                js.Formatting = Formatting.Indented;
#endif
                js.Serialize(jw, configFile);
            }
        }

        /// <summary>
        /// Cleans the links of empty sets. Sorts users by username and files by absPath.
        /// </summary>
        private void Clean()
        {
            var toRemove = this.links.Where((link) => link.Value.Count == 0).ToList();
            foreach (var link in toRemove)
                this.links.Remove(link.Key);

            this.files.Sort((l, r) => l.AbsPath.CompareTo(r.AbsPath));
            this.users.Sort((l, r) => l.Username.CompareTo(r.Username));
        }

        /// <summary>
        /// Validates that all files exist.
        /// </summary>
        /// <returns>Null if good, path to bad file if bad.</returns>
        public string ValidateFiles()
        {
            foreach (Json.File file in this.files)
            {
                if (!File.Exists(file.AbsPath))
                    return file.AbsPath;
            }
            return null;
        }

        /// <summary>
        /// Log the details of the currently running configuration to the console.
        /// </summary>
        public void LogDetails()
        {
            Logger.Log($"User count: {this.users.Count}");
            Logger.Log($"File count: {this.files.Count}");
            Logger.Log($"Link count: {this.links.Sum((linkset) => linkset.Value.Count)}");
        }
    }
}
