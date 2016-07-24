namespace Unlimitedinf.Fileserve.Tools
{
    using System;

    /// <summary>
    /// Log various operations of the server.
    /// </summary>
    internal static class Logger
    {
        internal static void ServerStart(string domain, int port)
        {
            Logger.Log($"Server started on {domain} at :{port}");
        }

        internal static void ServerRequest(string path, long ms)
        {
            Logger.Log($"Request for {path} handled in {ms}ms");
        }

        internal static void ServerRequestStart(string path)
        {
            Logger.Log($"Request for {path} started...");
        }

        internal static void ServerRequestStop(string path)
        {
            Logger.Log($"Request for {path} completed.");
        }

        internal static void ServerRequestKilled(string path)
        {
            Logger.Log($"Request for {path} killed.");
        }

        internal static void Log(string line)
        {
            Console.WriteLine(DateTime.Now.ToString("[yy-MM-dd HH:mm:ss.FFF] ") + line);
        }
    }
}
