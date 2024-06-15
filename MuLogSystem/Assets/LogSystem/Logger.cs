using System;

namespace LogSystem
{
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

    public static class Logger
    {
        private static LogWriter _logWriter;

        // ReSharper disable once MemberCanBePrivate.Global
        public static LogLevel LogOutputLevel { get; private set; } = LogLevel.Display;

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public static bool OpenShippingLog { get; private set; }

        public static void Initialize()
        {
            if (_logWriter == null)
            {
                _logWriter = new LogWriter();
                LogOutputLevel = LogSettings.Instance.logOutputLevel;
                OpenShippingLog = LogSettings.Instance.openShippingLog;
            }
        }
        
        public static void Uninitialize()
        {
            if (_logWriter != null)
            {
                _logWriter.Dispose();
                _logWriter = null;
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static void Log(LogLevel level, string message, params object[] args)
        {
            if (level > LogOutputLevel)
            {
                return;
            }

            string formattedMessage = string.Format(message, args);
            _logWriter.WriteLog(level, formattedMessage);

            if (level == LogLevel.Fatal)
            {
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