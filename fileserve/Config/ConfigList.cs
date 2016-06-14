namespace Unlimitedinf.Fileserve.Config
{
    using Json;
    using Properties;
    using System;

    internal sealed partial class Config
    {
        /// <summary>
        /// Lists all files in the configuration.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void FileList()
        {
            foreach (File file in this.files)
            {
                Console.WriteLine(Resources.ProgramConfigFileList, file.Id, file.WebPath, file.AbsPath);
            }
        }

        /// <summary>
        /// Lists all links in the configuration.
        /// </summary>
        public void LinkList()
        {
            foreach (var link in this.links)
            {
                Console.WriteLine(Resources.ProgramConfigLinkListUser, link.Key, this.UserIdToUsername(link.Key));
                foreach (var fileId in link.Value)
                {
                    Console.WriteLine(Resources.ProgramConfigLinkListFile, fileId, this.FileIdToAbsPath(fileId));
                }
            }
        }

        /// <summary>
        /// Lists all users in the configuration.
        /// </summary>
        public void UserList()
        {
            bool verbose = Tools.GetA.YesNo(Resources.GetAYesNoVerbose);

            foreach (User user in this.users)
            {
                if (verbose)
                    Console.WriteLine(Resources.ProgramConfigUserListVerbose,
                        user.Id,
                        user.Username,
                        user.ConcurrentFileLimit,
                        user.ByteRatePerFileLimit);
                else
                    Console.WriteLine(Resources.ProgramConfigUserList,
                        user.Id,
                        user.Username);
            }
        }
    }
}
