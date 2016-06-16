namespace Unlimitedinf.Fileserve.Config
{
    using Json;
    using Properties;
    using System;
    using System.Collections.Generic;

    internal sealed partial class Config
    {
        /// <summary>
        /// Add a file to the configuration.
        /// </summary>
        public void FileAdd()
        {
            string webPath = Tools.GetA.UrlPath(Resources.GetAUriFileWebPath);
            string absolutePath = Tools.GetA.FileAbsPath(Resources.GetAFileAbsPath);

            if (string.IsNullOrEmpty(webPath) || string.IsNullOrEmpty(absolutePath))
            {
                Console.WriteLine(Resources.ProgramConfigFileAddFail);
                return;
            }

            File file = new File()
            {
                WebPath = webPath,
                AbsPath = absolutePath,
                Id = Tools.Id.NewId(this.allIds)
            };
            this.files.Add(file);

            Console.WriteLine(Resources.ProgramConfigFileList, file.Id, file.WebPath, file.AbsPath);
        }

        /// <summary>
        /// Add a link to the configuration.
        /// </summary>
        public void LinkAdd()
        {
            Tools.Id userId = Tools.GetA.Id(Resources.GetAIdUser);
            Tools.Id fileId = Tools.GetA.Id(Resources.GetAIdFile);

            if (userId == Tools.Id.Empty || fileId == Tools.Id.Empty)
            {
                Console.WriteLine(Resources.ProgramConfigLinkAddFail);
                return;
            }

            if (!this.userIds.Contains(userId))
            {
                Console.WriteLine(Resources.ErrorUserIddNotFound);
                return;
            }
            if (!this.fileIds.Contains(fileId))
            {
                Console.WriteLine(Resources.ErrorFileIdNotFound);
                return;
            }

            if (!this.links.ContainsKey(userId))
                this.links[userId] = new HashSet<Tools.Id>();
            this.links[userId].Add(fileId);

            Console.WriteLine(Resources.ProgramConfigLinkAdd, userId, fileId);
        }

        /// <summary>
        /// Add a user to the configuration.
        /// </summary>
        public void UserAdd()
        {
            string username = Tools.GetA.String(Resources.GetAStringUser);
            string password = Tools.GetA.Password(Resources.GetAPasswordPass);
            uint? concurrentFileLimit = Tools.GetA.Uint(Resources.GetAUintConcurrentFileLimit1);
            uint? byteRatePerFileLimit = Tools.GetA.Uint(Resources.GetAUintByteRateLimitInf);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine(Resources.ProgramConfigUserAddFail);
                return;
            }

            User user = new User()
            {
                Username = username,
                PasswordHash = Tools.Password.Hash(password),
                ConcurrentFileLimit = concurrentFileLimit ?? UserDefaults.ConcurrentFileLimit,
                ByteRatePerFileLimit = byteRatePerFileLimit ?? UserDefaults.ByteRatePerFileLimit,
                Id = Tools.Id.NewId(this.allIds)
            };
            this.users.Add(user);

            Console.WriteLine(Resources.ProgramConfigUserAdd, user.Id, user.Username, user.ConcurrentFileLimit, user.ByteRatePerFileLimit);
        }
    }
}
