namespace Unlimitedinf.Fileserve.Config.Json
{
    using System;
    using System.Collections.Generic;
    using Tools;

    /// <summary>
    /// The main configuration contained by the config file.
    /// </summary>
    internal sealed class Overall
    {
        /// <summary>
        /// Valid users.
        /// </summary>
        public List<User> Users { get; set; }

        /// <summary>
        /// Valid files. Unique by Guid.
        /// </summary>
        public List<File> Files { get; set; }

        /// <summary>
        /// Map of which files each user has access to.
        /// </summary>
        public Dictionary<Id, HashSet<Id>> Links { get; set; }
    }

    /// <summary>
    /// An individual user's configuration.
    /// </summary>
    internal sealed class User
    {
        /// <summary>
        /// Usernames should be unique, but everything is handled by Guid.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// BCrypt password hash.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// The number of files one user is able to download at the same time.
        /// Should return a 429 Too Many Requests when hit. Defaults 1.
        /// </summary>
        public uint ConcurrentFileLimit { get; set; }

        /// <summary>
        /// The maximum speed allowed for each file for a user. Defaults unlimited.
        /// </summary>
        public uint ByteRatePerFileLimit { get; set; }

        public Id Id { get; set; }

        public User()
        {
            this.Id = Id.NewId();
            this.ConcurrentFileLimit = UserDefaults.ConcurrentFileLimit;
            this.ByteRatePerFileLimit = UserDefaults.ByteRatePerFileLimit;
        }
    }

    internal static class UserDefaults
    {
        public const uint ConcurrentFileLimit = 1;
        public const uint ByteRatePerFileLimit = 0;
    }

    /// <summary>
    /// The configuration for a file.
    /// </summary>
    internal sealed class File
    {
        /// <summary>
        /// The path after the base url corresponding to the file. This is looked up in the cache per user, so paths
        /// can easily conflict and point to completely different files.
        /// </summary>
        public string WebPath { get; set; }

        /// <summary>
        /// Absolute path in the filesystem for the file to serve.
        /// </summary>
        public string AbsPath { get; set; }

        public Id Id { get; set; }

        public File()
        {
            this.Id = Id.NewId();
        }
    }
}
