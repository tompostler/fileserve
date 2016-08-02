namespace Unlimitedinf.Fileserve.Tools
{
    using System;

    /// <summary>
    /// Log various operations of the server.
    /// </summary>
    internal static class Logger
    {
        internal static void ServerStart(string domain, int port)
            => Logger.Log($"Server started on {domain} at :{port}");

        internal static void ServerAuthRequest(string ip, long ms)
            => Logger.Log($"auth request from {ip} handled in {ms}ms");

        internal static void ServerRequest(string username, string path, long ms)
            => Logger.Log($"{username} wanting {path} handled in {ms}ms");

        internal static void ServerRequestStart(string username, string path)
            => Logger.Log($"{username} for {path} started...");

        internal static void ServerRequestPartial(string username, string path, byte percent)
            => Logger.Log($"{username} for {path} at {percent:#00}% complete");

        internal static void ServerRequestKilled(string username, string path)
            => Logger.Log($"{username} for {path} killed!");

        internal static void ServerRequestStop(string username, string path)
            => Logger.Log($"{username} for {path} completed");

        internal static void Server418(string username)
            => Logger.Log($"{username} is out for tea");

        internal static void Server429(string username)
            => Logger.Log($"{username} hit 429");

        internal static void ServerSemaphoreFullException(string username)
            => Logger.Log($"{username} affected by config reload");

        internal static void ConfigUsernameNotFound(string username)
            => Logger.Log($"Username '{username}' not found!");

        internal static void ConfigInvalidPassword(string username, string maskedPassword)
            => Logger.Log($"{username} failed password check {maskedPassword}!");

        internal static void Log(string line)
        {
            Console.WriteLine($"[{DateTime.Now:yy-MM-dd HH:mm:ss.fff}] "
                + $"[{System.Threading.Thread.CurrentThread.ManagedThreadId:00}] "
                + line);
        }
    }
}
