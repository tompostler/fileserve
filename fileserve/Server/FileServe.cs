namespace Unlimitedinf.Fileserve.Server
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using Tools;

    internal sealed class FileServe : FileServer
    {
        private Config.Config config;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="config"></param>
        public FileServe(Config.Config config) : base()
        {
            this.config = config;
        }

        /// <summary>
        /// Actually process an HTTP request while respecting the constraints in the configuration.
        /// </summary>
        /// <param name="context"></param>
        protected override void Process(HttpListenerContext context)
        {
            // Parse the id from the authorization or make the user authorize
            Id userId = this.Authorized(context.Request.Headers["Authorization"]);
            if (userId == Id.Empty)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.AddHeader("WWW-Authenticate", "Basic realm=\"User Visible Realm\"");
                context.Response.OutputStream.Close();
                return;
            }

            // Give user the directory
            string url = context.Request.RawUrl.Substring(1);
            if (string.IsNullOrEmpty(url))
            {
                using (StreamWriter sw = new StreamWriter(context.Response.OutputStream))
                {
                    sw.Write(Html.FilesToHtml(this.config.FilesAvailableToUser(userId), this.config.UserIdToUsername(userId)));
                }
            }

            else if (url == "favicon.ico")
            {
                Properties.Resources.unlimitedinf.Save(context.Response.OutputStream);
                context.Response.OutputStream.Close();
                return;
            }

            else if (this.config.ValidUserAccess(userId, url))
            {
                //TODO serve file
            }
        }

        /// <summary>
        /// Check if a request has an authorized user.
        /// </summary>
        /// <param name="authorizationHeader"></param>
        /// <returns><see cref="Id.Empty"/> if not valid, <see cref="Id"/> otherwise.</returns>
        private Id Authorized(string authorizationHeader)
        {
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Basic "))
                return Id.Empty;

            string b64 = authorizationHeader.Substring(6);
            string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(b64));
            string[] parts = decoded.Split(':');

            if (parts.Length != 2)
                return Id.Empty;

            return this.config.ValidateUserToId(parts[0], parts[1]);
        }
    }
}
