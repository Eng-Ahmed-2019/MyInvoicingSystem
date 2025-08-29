using System;
using System.IO;

namespace InvoicingSystem.Services
{
    public class FileLoggerService
    {
        private readonly string _logFilePath;

        public FileLoggerService()
        {
            _logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "system-log.txt");

            var logDir = Path.GetDirectoryName(_logFilePath);
            if(!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
        }

        public void Log(string message,string level = "INFO")
        {
            using(var writer = new StreamWriter(_logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}");
            }
        }

        public void LogError(Exception ex)
        {
            Log($"ERROR: {ex.Message} | StackTrace: {ex.StackTrace}", "ERROR");
        }
    }
}