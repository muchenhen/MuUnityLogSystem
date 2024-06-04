// 定义日志宏

using System;

namespace LogSystem
{
    public static class Logger
    {
        // 日志等级枚举
        public enum LogLevel
        {
            Fatal,
            Error,
            Warning,
            Display,
            Log,
            Verbose,
            VeryVerbose
        }

        // 日志写入器
        private static LogWriter _logWriter;

        // 初始化日志系统
        public static void Initialize()
        {
            _logWriter = new LogWriter();
        }

        // 写入日志
        // ReSharper disable once MemberCanBePrivate.Global
        public static void Log(LogLevel level, string message, params object[] args)
        {
            string formattedMessage = string.Format(message, args);
            _logWriter.WriteLog(level, formattedMessage);

            if (level == LogLevel.Fatal)
            {
                // Crash the application
                throw new FatalException(formattedMessage);
            }
        }

        public static void LogDisplay(string message, params object[] args)
        {
            Log(LogLevel.Display, message, args);
        }

        public static void LogWarning(string message, params object[] args)
        {
            Log(LogLevel.Warning, message, args);
        }

        public static void LogError(string message, params object[] args)
        {
            Log(LogLevel.Error, message, args);
        }
    }

    public class FatalException : Exception
    {
        public FatalException(string message) : base(message)
        {
        }
    }
}