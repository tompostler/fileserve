namespace Unlimitedinf.Fileserve.Config
{
    using Json;

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
                return Tools.Id.Empty;

            foreach (User user in this.users)
                if (user.Username == username)
                    return Tools.Password.Validate(password, user.PasswordHash) ? user.Id : Tools.Id.Empty;

            return Tools.Id.Empty;
        }
    }
}
