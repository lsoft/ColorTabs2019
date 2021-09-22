using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColorTabs2019
{
    /// <summary>
    /// Logging helpers.
    /// Taken from  https://github.com/bert2/microscope completely.
    /// Take a look to that repo, it's amazing!
    /// </summary>
    public static class Logging
    {
        private static readonly object _locker = new object();

        // Logs go to: C:\Users\<user>\AppData\Local\Temp\colortabs2019.log
        // We're using one log file for each process to prevent concurrent file access.
        private static readonly string vsLogFile = $"{Path.GetTempPath()}/colortabs2019.log";

        static Logging()
        {
            if (File.Exists(vsLogFile))
            {
                try
                {
                    File.Delete(vsLogFile);
                }
                catch
                {
                    //we do nothing here
                }
            }
        }

        [Conditional("DEBUG")]
        public static void LogVS(
            object? data = null,
            [CallerFilePath] string? file = null,
            [CallerMemberName] string? method = null)
            => Log(vsLogFile, file!, method!, data);
        

        public static void Log(
            string logFile,
            string callingFile,
            string callingMethod,
            object? data = null)
        {
            lock (_locker)
            {
                File.AppendAllText(
                    logFile,
                    $"{DateTime.Now:HH:mm:ss.fff} "
                    + $"{Process.GetCurrentProcess().Id,5} "
                    + $"{Thread.CurrentThread.ManagedThreadId,3} "
                    + $"{Path.GetFileNameWithoutExtension(callingFile)}.{callingMethod}()"
                    + $"{(data == null ? "" : $": {data}")}\n");
            }
        }
    }
}
