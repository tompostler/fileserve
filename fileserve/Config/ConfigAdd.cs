namespace Unlimitedinf.Fileserve.Config
{
    using Json;
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
                AbsPath = absolutePath
            };
            this.files.Add(file);

            Console.WriteLine(Resources.ProgramConfigFileAdd, file.Id, file.WebPath, file.AbsPath);
        }

        /// <summary>
        /// Add a link to the configuration.
        /// </summary>
        public void LinkAdd()
        {
            Guid userId = Tools.GetA.Guid(Resources.GetAGuidUser);
            Guid fileId = Tools.GetA.Guid(Resources.GetAGuidFile);

            if (userId == Guid.Empty || fileId == Guid.Empty)
            {
                Console.WriteLine(Resources.ProgramConfigLinkAddFail);
                return;
            }

            if (!this.links.ContainsKey(userId))
                this.links[userId] = new HashSet<Guid>();
            this.links[userId].Add(fileId);

            Console.WriteLine(Resources.ProgramConfigLinkAdd, userId, fileId);
        }

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
    }
}
