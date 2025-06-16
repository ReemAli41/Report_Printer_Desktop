using System;
using System.IO;
using System.Text;

namespace Report_Printer_Desktop.Helpers
{
    public class Helper_Logger
    {
        private readonly string _loggerName;
        private readonly string _logFilePath;
        private static readonly object _lock = new object();

        private Helper_Logger(string loggerName)
        {
            _loggerName = loggerName;
            string logDirectory = @"C:\Logs";
            Directory.CreateDirectory(logDirectory);

            _logFilePath = Path.Combine(logDirectory, $"{_loggerName}_{DateTime.Now:yyyyMMdd}.log");
        }

        public static Helper_Logger Create(string loggerName)
        {
            return new Helper_Logger(loggerName);
        }

        public void LogInfo(string message)
        {
            WriteLog("INFO", message);
        }

        public void LogException(Exception ex, string context = null)
        {
            var fullMessage = context != null ? $"{context}: {ex}" : ex.ToString();
            WriteLog("ERROR", fullMessage);
        }

        private void WriteLog(string level, string message)
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {level} [{_loggerName}]: {message}";

            lock (_lock)
            {
                try
                {
                    File.AppendAllText(_logFilePath, logMessage + Environment.NewLine, Encoding.UTF8);
                }
                catch
                {
                    
                }
            }

            Console.WriteLine(logMessage);
        }
    }
}
