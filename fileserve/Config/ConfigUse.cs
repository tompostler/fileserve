namespace Unlimitedinf.Fileserve.Config
{
    using Json;
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal sealed partial class Config
    {
        /// <summary>
        /// Given a username and password, validate that the user exists and has supplied the correct password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Tools.Id ValidateUserToId(string username, string password)
        {
            if (!this.usernames.Contains(username))
            {
                Tools.Logger.ConfigUsernameNotFound(username);
                return Tools.Id.Empty;
            }

            foreach (User user in this.users)
                if (user.Username == username)
                {
                    Tools.Id userId = Tools.Password.Validate(password, user.PasswordHash) ? user.Id : Tools.Id.Empty;
                    if (userId == Tools.Id.Empty)
                        Tools.Logger.ConfigInvalidPassword(username, "".PadLeft(password.Length, '*'));
                    return userId;
                }

            return Tools.Id.Empty;
        }

        /// <summary>
        /// Given a user id, return the appropriately typed container of all the files they have access to.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Tuple<string, FileInfo>> FilesAvailableToUser(Tools.Id userId)
        {
            List<Tuple<string, FileInfo>> directory = new List<Tuple<string, FileInfo>>();

            if (!this.links.ContainsKey(userId) || this.links[userId].Count == 0)
                return directory;

            foreach (Tools.Id fileId in this.links[userId])
                directory.Add(new Tuple<string, FileInfo>(this.filesById[fileId].WebPath, new FileInfo(this.filesById[fileId].AbsPath)));
            return directory;
        }

        /// <summary>
        /// Convert a fild Id to absolute path.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string FileIdToAbsPath(Tools.Id id) => this.filesById[id].AbsPath;

        /// <summary>
        /// Convert a fild Id to web path.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string FileIdToWebPath(Tools.Id id) => this.filesById[id].WebPath;

        /// <summary>
        /// Convert a user Id to username.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string UserIdToUsername(Tools.Id id) => this.usersById[id].Username;

        /// <summary>
        /// Convert a file's web path to its Id.
        /// </summary>
        /// <param name="webPath"></param>
        /// <returns></returns>
        public Tools.Id FileWebPathToId(string webPath)
        {
            Json.File file = this.files.Find((f) => f.WebPath == webPath);
            if (string.IsNullOrEmpty(file.WebPath) && string.IsNullOrEmpty(file.AbsPath))
                return Tools.Id.Empty;
            return file.Id;
        }

        /// <summary>
        /// Given a user Id and file Id, validate that both exist and the file is accessible to the user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public bool ValidUserAccess(Tools.Id userId, Tools.Id fileId)
        {
            return this.userIds.Contains(userId)
                && this.fileIds.Contains(fileId)
                && this.links[userId].Contains(fileId);
        }

        /// <summary>
        /// Given a user Id and file web path, validate that both exist and the file is accessible to the user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public bool ValidUserAccess(Tools.Id userId, string webPath)
        {
            return this.ValidUserAccess(userId, this.FileWebPathToId(webPath));
        }

        /// <summary>
        /// Given a user Id, get the byte rate per file limit.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public long UserIdToTransferRate(Tools.Id userId) => this.usersById[userId].ByteRatePerFileLimit;

        /// <summary>
        /// Given a user Id, get the concurrency limit for that user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public uint UserIdToConcurrencyLimit(Tools.Id userId) => this.usersById[userId].ConcurrentFileLimit;

        /// <summary>
        /// Given a web path (and assuming it's been validated), convert it to the backing fileinfo.
        /// </summary>
        /// <param name="webPath"></param>
        /// <returns></returns>
        public FileInfo FileWebPathToFileInfo(string webPath) 
            => new FileInfo(this.filesById[this.FileWebPathToId(webPath)].AbsPath);

        /// <summary>
        /// Given a user Id, see if it exists in the config.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UserIdExists(Tools.Id userId) => this.userIds.Contains(userId);
    }
}
