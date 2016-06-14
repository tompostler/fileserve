﻿namespace Unlimitedinf.Fileserve.Config
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

        private HashSet<Id> fileIds => new HashSet<Id>(this.files.ConvertAll((file) => file.Id));
        private HashSet<Id> userIds => new HashSet<Id>(this.users.ConvertAll((user) => user.Id));
        private HashSet<Id> allIds => new HashSet<Id>(this.fileIds.Union(this.userIds));
        
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

                StreamReader sr = new StreamReader(filename);
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
                this.links = new Dictionary<Id, HashSet<Id>>();
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

            StreamWriter sw = new StreamWriter(filename);
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                JsonSerializer js = new JsonSerializer();
                if (prettify)
                    js.Formatting = Formatting.Indented;
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
        /// Convert a fild Id to absolute path. Does not check existence.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string FileIdToAbsPath(Id id)
        {
            return this.files.Find((file) => file.Id == id).AbsPath;
        }

        /// <summary>
        /// Convert a user Id to username. Does not check existence.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string UserIdToUsername(Id id)
        {
            return this.users.Find((user) => user.Id == id).Username;
        }
    }
}
