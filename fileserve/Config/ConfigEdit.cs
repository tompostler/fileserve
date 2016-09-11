namespace Unlimitedinf.Fileserve.Config
{
    using Json;
    using Properties;
    using System;
    using Unlimitedinf.Tools;

    internal sealed partial class Config
    {
        /// <summary>
        /// Edit a file in the configuration.
        /// </summary>
        public void FileEdit()
        {
            // Find the file
            Id id = Tools.GetA.Id(Resources.GetAId);
            if (id == Id.Empty)
            {
                Console.WriteLine(Resources.ProgramConfigFileEditFail);
                return;
            }
            File file = this.files.Find((f) => f.Id == id);
            if (file == null)
            {
                Console.WriteLine(Resources.ErrorIdNotFound);
                return;
            }
            Console.WriteLine(Resources.ProgramConfigFileList, file.Id, file.WebPath, file.AbsPath);

            // Get updates
            string webPath = Tools.GetA.UrlPath(Resources.GetAUriFileWebPath);
            string absPath = Tools.GetA.FileAbsPath(Resources.GetAFileAbsPath);

            if (string.IsNullOrEmpty(webPath) || string.IsNullOrEmpty(absPath))
            {
                Console.WriteLine(Resources.ProgramConfigFileEditFail);
                return;
            }

            // Update
            file.WebPath = webPath;
            file.AbsPath = absPath;

            Console.WriteLine(Resources.ProgramConfigFileList, file.Id, file.WebPath, file.AbsPath);
        }

        /// <summary>
        /// Edit a user in the configuration.
        /// </summary>
        public void UserEdit()
        {
            // Find the user
            Id id = Tools.GetA.Id(Resources.GetAId);
            if (id == Id.Empty)
            {
                Console.WriteLine(Resources.ProgramConfigUserEditFail);
                return;
            }
            User user = this.users.Find((f) => f.Id == id);
            if (user == null)
            {
                Console.WriteLine(Resources.ErrorIdNotFound);
                return;
            }
            Console.WriteLine(Resources.ProgramConfigUserListVerbose, user.Id, user.Username, user.ConcurrentFileLimit, user.ByteRatePerFileLimit);

            // Get updates
            string username = Tools.GetA.String(Resources.GetAStringUser);
            string password = Tools.GetA.Password(Resources.GetAPasswordPass);
            uint? concurrentFileLimit = Tools.GetA.Uint(Resources.GetAUintConcurrentFileLimit1);
            uint? byteRatePerFileLimit = Tools.GetA.Uint(Resources.GetAUintByteRateLimitInf);
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine(Resources.ProgramConfigUserEditFail);
                return;
            }

            // Update
            user.Username = username;
            user.PasswordHash = Tools.Password.Hash(password);
            user.ConcurrentFileLimit = concurrentFileLimit ?? UserDefaults.ConcurrentFileLimit;
            user.ByteRatePerFileLimit = byteRatePerFileLimit ?? UserDefaults.ByteRatePerFileLimit;

            Console.WriteLine(Resources.ProgramConfigUserListVerbose, user.Id, user.Username, user.ConcurrentFileLimit, user.ByteRatePerFileLimit);
        }
    }
}
