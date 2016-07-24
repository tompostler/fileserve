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

        internal static void ServerAuthRequest(string ip, long ms)
        {
            Logger.Log($"auth request from {ip} handled in {ms}ms");
        }

        internal static void ServerRequest(Id userId, string path, long ms)
        {
            Logger.Log($"{userId} wanting {path} handled in {ms}ms");
        }

        internal static void ServerRequestStart(Id userId, string path)
        {
            Logger.Log($"{userId} for {path} started...");
        }

        internal static void ServerRequestStop(Id userId, string path)
        {
            Logger.Log($"{userId} for {path} completed.");
        }

        internal static void ServerRequestKilled(Id userId, string path)
        {
            Logger.Log($"{userId} for {path} killed.");
        }

        internal static void Log(string line)
        {
            Console.WriteLine(DateTime.Now.ToString("[yy-MM-dd HH:mm:ss.FFF] ") 
                + $"[{System.Threading.Thread.CurrentThread.ManagedThreadId}] " 
                + line);
        }
    }
}
