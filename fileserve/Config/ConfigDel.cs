namespace Unlimitedinf.Fileserve.Config
{
    using Json;
    using Properties;
    using System;

    internal sealed partial class Config
    {
        /// <summary>
        /// Delete a file from the configuration. Cascade deletion through links.
        /// </summary>
        public void FileDel()
        {
            Tools.Id id = Tools.GetA.Id(Resources.GetAId);
            if (id == Tools.Id.Empty)
            {
                Console.WriteLine(Resources.ProgramConfigFileDelFail);
                return;
            }

            int index = this.files.FindIndex((f) => f.Id == id);
            if (index == -1)
            {
                Console.WriteLine(Resources.ErrorIdNotFound);
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
            Tools.Id userId = Tools.GetA.Id(Resources.GetAIdUser);
            Tools.Id fileId = Tools.GetA.Id(Resources.GetAIdFile);

            if (userId == Tools.Id.Empty || fileId == Tools.Id.Empty)
            {
                Console.WriteLine(Resources.ProgramConfigLinkDelFail);
                return;
            }

            if (!this.links.ContainsKey(userId))
            {
                Console.WriteLine(Resources.ErrorUserIddNotFound);
                return;
            }
            if (!this.links[userId].Contains(fileId))
            {
                Console.WriteLine(Resources.ErrorFileIdNotFound);
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
            Tools.Id id = Tools.GetA.Id(Resources.GetAId);
            if (id == Tools.Id.Empty)
            {
                Console.WriteLine(Resources.ProgramConfigUserDelFail);
                return;
            }

            int index = this.users.FindIndex((f) => f.Id == id);
            if (index == -1)
            {
                Console.WriteLine(Resources.ErrorIdNotFound);
                return;
            }

            User user = this.users[index];
            this.users.RemoveAt(index);
            this.links.Remove(id);

            Console.WriteLine(Resources.ProgramConfigUserDel, user.Username);
        }
    }
}
