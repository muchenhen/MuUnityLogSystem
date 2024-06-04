using System;

namespace LogSystem
{
    public static class Logger
    {
        // 日志等级枚举
        public enum LogLevel
        {
            /// <summary>
            /// Always prints a fatal error to console (and log file) and crashes (even if logging is disabled)
            /// </summary>
            Fatal,

            /// <summary>
            /// Prints an error to console (and log file).
            /// Commandlets and the editor collect and report errors. Error messages result in commandlet failure.
            /// </summary>
            Error,

            /// <summary>
            /// Prints a warning to console (and log file).
            /// Commandlets and the editor collect and report warnings. Warnings can be treated as an error.
            /// </summary>
            Warning,

            /// <summary>
            /// Prints a message to console (and log file)
            /// </summary>
            Display,

            /// <summary>
            /// Prints a message to a log file (does not print to console)
            /// </summary>
            Log,

            /// <summary>
            /// Prints a verbose message to a log file (if Verbose logging is enabled for the given category,
            /// usually used for detailed logging)
            /// </summary>
            Verbose,

            /// <summary>
            /// Prints a verbose message to a log file (if VeryVerbose logging is enabled,
            /// usually used for detailed logging that would otherwise spam output)
            /// </summary>
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
    }

    public class FatalException : Exception
    {
        public FatalException(string message) : base(message)
        {
        }
    }
}