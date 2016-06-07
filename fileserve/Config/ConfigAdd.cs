﻿namespace Unlimitedinf.Fileserve.Config
{
    using Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal sealed partial class Config
    {
        /// <summary>
        /// Add a user to the configuration.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="passwordHash"></param>
        /// <param name="concurrentFileLimit"></param>
        /// <param name="byteRatePerFileLimit"></param>
        public void UserAdd(string username, string passwordHash,
            int concurrentFileLimit = UserDefaults.ConcurrentFileLimit,
            int byteRatePerFileLimit = UserDefaults.ByteRatePerFileLimit)
        {
            User user = new User()
            {
                Username = username,
                PasswordHash = passwordHash,
                ConcurrentFileLimit = concurrentFileLimit,
                ByteRatePerFileLimit = byteRatePerFileLimit
            };
            this.users.Add(user);
        }

        /// <summary>
        /// Add a file to the configuration.
        /// </summary>
        /// <param name="webPath"></param>
        /// <param name="absolutePath"></param>
        /// TODO: verify webPath is a valid URL string
        /// TODO: verify absolutePath has correct permissions/exists/etc
        public void FileAdd(string webPath, string absolutePath)
        {
            File file = new File()
            {
                WebPath = webPath,
                AbsolutePath = absolutePath
            };
            this.files.Add(file);
        }
    }
}
