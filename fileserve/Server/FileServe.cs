namespace Unlimitedinf.Fileserve.Server
{
    using System;
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
            Id id = this.Authorized(context.Request.Headers["Authorization"]);
            if (id == Id.Empty)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.AddHeader("WWW-Authenticate", "Basic realm=\"User Visible Realm\"");
                context.Response.OutputStream.Close();
                return;
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
