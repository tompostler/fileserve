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
        /// Delete a file from the configuration. Cascade deletion through links.
        /// </summary>
        public void FileDel()
        {
            Guid id = Tools.GetA.Guid(Resources.GetAGuid);
            if (id == Guid.Empty)
            {
                Console.WriteLine(Resources.ProgramConfigFileDelFail);
                return;
            }

            int index = this.files.FindIndex((f) => f.Id == id);
            if (index == -1)
            {
                Console.WriteLine(Resources.ErrorGuidNotFound);
                return;
            }

            File file = this.files[index];
            this.files.RemoveAt(index);
            foreach (var link in this.links)
            {
                if (link.Value.Contains(id))
                    link.Value.Remove(id);
            }

            Console.WriteLine(Resources.ProgramConfigFileDel, file.WebPath, file.AbsPath);
        }

        /// <summary>
        /// Delete a link from the configuration.
        /// </summary>
        public void LinkDel()
        {
            Guid userId = Tools.GetA.Guid(Resources.GetAGuidUser);
            Guid fileId = Tools.GetA.Guid(Resources.GetAGuidFile);

            if (userId == Guid.Empty || fileId == Guid.Empty)
            {
                Console.WriteLine(Resources.ProgramConfigLinkDelFail);
                return;
            }

            if (!this.links.ContainsKey(userId))
            {
                Console.WriteLine(Resources.ErrorUserGuidNotFound);
                return;
            }
            if (!this.links[userId].Contains(fileId))
            {
                Console.WriteLine(Resources.ErrorFileGuidNotFound);
                return;
            }

            this.links[userId].Remove(fileId);
            Console.WriteLine(Resources.ProgramConfigLinkDel, userId, this.UserIdToUsername(userId), fileId, this.FileIdToAbsPath(fileId));
        }

        /// <summary>
        /// Delete a user from the configuration. Cascade deletion through links.
        /// </summary>
        public void UserDel()
        {
            Guid id = Tools.GetA.Guid(Resources.GetAGuid);
            if (id == Guid.Empty)
            {
                Console.WriteLine(Resources.ProgramConfigUserDelFail);
                return;
            }

            int index = this.users.FindIndex((f) => f.Id == id);
            if (index == -1)
            {
                Console.WriteLine(Resources.ErrorGuidNotFound);
                return;
            }

            User user = this.users[index];
            this.users.RemoveAt(index);
            this.links.Remove(id);

            Console.WriteLine(Resources.ProgramConfigUserDel, user.Username);
        }
    }
}
