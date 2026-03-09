using System;
using System.IO;
using System.Text;

namespace MedScheduler
{
    public static class Logger
    {
        private static readonly object _gate = new();
        private static readonly string _path = "scheduler.log";

		//Create a public static void for Info, Warn and Error that accept a string message. 
		//For each one, call Write() and pass in the level and message variable
        public static void Info(string msg) => Write("INFO", msg);
        public static void Warn(string msg) => Write("WARN", msg);
        public static void Error(string msg) => Write("ERROR", msg);

        private static void Write(string level, string msg)
        {
            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {level}: {msg}";
            lock (_gate)
            {
                try
                {
                    using var sw = new StreamWriter(_path, append: true, Encoding.UTF8);
                    sw.WriteLine(line);
                }
                catch
                {
                    // As a last resort, avoid crashing the app because logging failed.
                    Console.Error.WriteLine("LOGGING FAILURE: " + line);
                }
            }
        }
    }
}
