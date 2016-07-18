namespace Unlimitedinf.Fileserve.Server
{
    using System;
    using System.Net;

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
            throw new NotImplementedException();
        }
    }
}
