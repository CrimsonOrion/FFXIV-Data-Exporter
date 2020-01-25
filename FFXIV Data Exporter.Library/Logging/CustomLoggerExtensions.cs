using System;
using System.Diagnostics;
using System.IO;

namespace FFXIV_Data_Exporter.Library.Logging
{
    public static class CustomLoggerExtensions
    {
        public static void Log(this ICustomLogger logger, string message, params object[] args)
        {
            var line = $"<{DateTime.Now.ToLongTimeString()}> : {message}";
            if (logger.OutputToConsole)
                Console.WriteLine(line);
        }

        public static void LogCritical(this ICustomLogger logger, Exception exception, string message, params object[] args)
        {
            var exceptionMessage = $"Exception Message:{exception.Message}";
            var innerException = $"Inner Message:{exception.InnerException}";
            var stackTrace = $"Stack Trace:{exception.StackTrace}";
            var line = $"[Critical] <{DateTime.Now.ToLongTimeString()}> : {message}\r\n\t{exceptionMessage}\r\n\t{innerException}\r\n\t{stackTrace}";
            File.AppendAllText(logger.LogFileInfo.FullName, line + $"\r\n");
            if (logger.OutputToConsole)
                Console.WriteLine(line);
        }

        public static void LogDebug(this ICustomLogger logger, string message, params object[] args)
        {
            var line = $"[Debug] <{DateTime.Now.ToLongTimeString()}> : {message}";
            Debug.WriteLine(line);
        }

        public static void LogError(this ICustomLogger logger, Exception exception, string message, params object[] args)
        {
            var exceptionMessage = $"Exception Message:{exception.Message}";
            var innerException = $"Inner Message:{exception.InnerException}";
            var line = $"[Error] <{DateTime.Now.ToLongTimeString()}> : {message}\r\n\t{exceptionMessage}\r\n\t{innerException}";
            File.AppendAllText(logger.LogFileInfo.FullName, line + $"\r\n");
            if (logger.OutputToConsole)
                Console.WriteLine(line);
        }

        public static void LogInformation(this ICustomLogger logger, string message, params object[] args)
        {
            var line = $"[Information] <{DateTime.Now.ToLongTimeString()}> : {message}";
            File.AppendAllText(logger.LogFileInfo.FullName, line + $"\r\n");
            if (logger.OutputToConsole)
                Console.WriteLine(line);
        }

        public static void LogTrace(this ICustomLogger logger, Exception exception, string message, params object[] args)
        {
            var exceptionMessage = $"Exception Message:{exception.Message}";
            var innerException = $"Inner Message:{exception.InnerException}";
            var stackTrace = $"Stack Trace:{exception.StackTrace}";
            var line = $"[Trace] <{DateTime.Now.ToLongTimeString()}> : {message}\r\n\t{exceptionMessage}\r\n\t{innerException}\r\n\t{stackTrace}";
            File.AppendAllText(logger.LogFileInfo.FullName, line + $"\r\n");
            if (logger.OutputToConsole)
                Console.WriteLine(line);
        }

        public static void LogWarning(this ICustomLogger logger, Exception exception, string message, params object[] args)
        {
            var exceptionMessage = $"Exception Message:{exception.Message}";
            var line = $"[Warning] <{DateTime.Now.ToLongTimeString()}> : {message}\r\n\t{exceptionMessage}";
            File.AppendAllText(logger.LogFileInfo.FullName, line + $"\r\n");
            if (logger.OutputToConsole)
                Console.WriteLine(line);
        }
    }
}