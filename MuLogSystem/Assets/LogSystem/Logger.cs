using System;
using System.Diagnostics;
using System.Text;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

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
        private static DateTime _lastLogTime;
        private static string _lastLogTimeString;

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


            if (DateTime.Now - _lastLogTime > TimeSpan.FromMilliseconds(1))
            {
                _lastLogTime = DateTime.Now;
                _lastLogTimeString = string.Format("{0:yyyy.MM.dd-HH.mm.ss:fff}", _lastLogTime);
            }

            string timestamp = _lastLogTimeString;

            StringBuilder sb = new StringBuilder();
            sb.Append("[").Append(timestamp).Append("][").Append(logLevelString).Append("] ").Append(message);
            message = sb.ToString();

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
            Profiler.BeginSample("Logger.LogDisplay");
            Log(LogLevel.Display, message);
            Profiler.EndSample();
        }

        public static void LogWarning(string message)
        {
            Profiler.BeginSample("Logger.LogWarning");
            Log(LogLevel.Warning, message);
            Profiler.EndSample();
        }

        public static void LogError(string message)
        {
            Profiler.BeginSample("Logger.LogError");
            Log(LogLevel.Error, message);
            Profiler.EndSample();
        }
    }

    public class FatalException : Exception
    {
        public FatalException(string message) : base(message)
        {
        }
    }
}