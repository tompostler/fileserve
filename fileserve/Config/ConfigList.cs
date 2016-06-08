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
        /// <param name="userId">Only list the linked files for this user.</param>
        /// <param name="prettify">List the usernames and absolute paths in addition to the guids.</param>
        public void LinkList(Guid userId = default(Guid), bool prettify = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lists all users in the configuration.
        /// </summary>
        /// <param name="userId">Only list one user.</param>
        /// <param name="detailed">Display the whole user information instead of just usernames.</param>
        public void UserList(Guid userId = default(Guid), bool detailed = false)
        {
            throw new NotImplementedException();
        }
    }
}
