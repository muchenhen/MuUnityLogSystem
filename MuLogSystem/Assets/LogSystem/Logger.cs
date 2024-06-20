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
        public static void Log(LogLevel level, string message)
        {
            if (level > LogOutputLevel)
            {
                return;
            }

            string logLevelString = level switch
            {
                LogLevel.Log => "Log",
                LogLevel.Display => "Display",
                LogLevel.Warning => "Warning",
                LogLevel.Error => "Error",
                LogLevel.Fatal => "Fatal",
                LogLevel.Verbose => "Verbose",
                LogLevel.VeryVerbose => "VeryVerbose",
                _ => "Unknown"
            };

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
                case LogLevel.Fatal:
                    break;
                case LogLevel.Log:
                    break;
                case LogLevel.Verbose:
                    break;
                case LogLevel.VeryVerbose:
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

        public static void LogDisplay(string message)
        {
            Log(LogLevel.Display, message);
        }

        public static void LogWarning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        public static void LogError(string message)
        {
            Log(LogLevel.Error, message);
        }
    }

    public class FatalException : Exception
    {
        public FatalException(string message) : base(message)
        {
        }
    }
}