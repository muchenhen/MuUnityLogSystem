using System;
using UnityEngine;

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

            message = string.Format(message, args);
            string logLevelString = GetLogLevelString(level);
            string timestamp = DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss:fff");
            message = $"[{timestamp}][{logLevelString}] {message}";
            _logWriter.WriteLog(level, message);
#if UNITY_EDITOR
            switch (level)
            {
                case LogLevel.Display:
                    Debug.Log(message);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogLevel.Error:
                    Debug.LogError(message);
                    break;
            }
#elif DEVELOPMENT_BUILD
            if (level != Logger.LogLevel.Log)
            {
                UnityEngine.Debug.Log(logEntry);
            }
#else
            if (Logger.OpenShippingLog && level >= Logger.LogOutputLevel)
            {
                UnityEngine.Debug.Log(logEntry);
            }
#endif

            if (level == LogLevel.Fatal)
            {
                throw new FatalException(message);
            }
        }

        private static string GetLogLevelString(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Log:
                    return "Log";
                case LogLevel.Display:
                    return "Display";
                case LogLevel.Warning:
                    return "Warning";
                case LogLevel.Error:
                    return "Error";
                default:
                    return "Unknown";
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