namespace Unlimitedinf.Fileserve.Server
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;
    using Tools;

    internal sealed class FileServe : FileServer
    {
        private Config.Config config;
        private Dictionary<Id, SemaphoreSlim> concurrentFileLimit;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="config"></param>
        public FileServe(Config.Config config, int port) : base(port)
        {
            this.config = config;
            this.concurrentFileLimit = new Dictionary<Id, SemaphoreSlim>();
        }

        public override void Start()
        {
            base.Start();
            this.config.LogDetails();
        }

        /// <summary>
        /// Actually process an HTTP request while respecting the constraints in the configuration.
        /// </summary>
        /// <param name="context"></param>
        protected override void Process(HttpListenerContext context)
        {
            Stopwatch duration = Stopwatch.StartNew();

            // Parse the id from the authorization or make the user authorize
            Id userId = this.Authorized(context.Request.Headers["Authorization"]);
            if (userId == Id.Empty)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.AddHeader("WWW-Authenticate", "Basic realm=\"User Visible Realm\"");
                context.Response.OutputStream.Close();
                Logger.ServerAuthRequest(context.Request.RemoteEndPoint.Address.ToString(), duration.ElapsedMilliseconds);
                return;
            }

            // Give user the directory
            string url = context.Request.RawUrl.Substring(1);
            if (string.IsNullOrEmpty(url))
            {
                using (StreamWriter sw = new StreamWriter(context.Response.OutputStream))
                {
                    sw.Write(Html.FilesToHtml(this.config.FilesAvailableToUser(userId), this.config.UserIdToUsername(userId)));
                    Logger.ServerRequest(userId, "/", duration.ElapsedMilliseconds);
                    return;
                }
            }

            else if (url == "favicon.ico")
            {
                Properties.Resources.unlimitedinf.Save(context.Response.OutputStream);
                context.Response.OutputStream.Close();
                Logger.ServerRequest(userId, "favicon.ico", duration.ElapsedMilliseconds);
                return;
            }

            // Serve up the file
            else if (this.config.ValidUserAccess(userId, url))
            {
                if (!this.concurrentFileLimit.ContainsKey(userId))
                    this.concurrentFileLimit[userId] = new SemaphoreSlim((int)this.config.UserIdToConcurrencyLimit(userId));
                FileInfo file = this.config.FileWebPathToFileInfo(url);

                // Hit the limit
                if (this.concurrentFileLimit[userId].CurrentCount == 0)
                {
                    context.Response.StatusCode = 429; //Too many requests
                    context.Response.OutputStream.Close();
                    Logger.Log($"429 for {userId}");
                    return;
                }
                this.concurrentFileLimit[userId].Wait();

                context.Response.ContentType = "application/octet-stream";
                context.Response.ContentLength64 = file.Length;
                using (ThrottledStream ts = new ThrottledStream(context.Response.OutputStream, this.config.UserIdToTransferRate(userId)))
                using (FileStream input = file.OpenRead())
                {
                    Logger.ServerRequestStart(userId, file.FullName);
                    byte[] buffer = new byte[1024 * 64];
                    int nbytes;
                    while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        try
                        {
                            ts.Write(buffer, 0, nbytes);
                        }
                        catch (HttpListenerException)
                        {
                            Logger.ServerRequestKilled(userId, file.FullName);
                            break;
                        }
                    }
                    Logger.ServerRequestStop(userId, file.FullName);
                }
                context.Response.OutputStream.Close();
                this.concurrentFileLimit[userId].Release();
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
