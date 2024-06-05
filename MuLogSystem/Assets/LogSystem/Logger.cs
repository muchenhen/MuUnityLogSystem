// 定义日志宏

using System;
using UnityEditor;

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

        // 日志输出等级
        private static LogLevel LogOutputLevel { get; set; } = LogLevel.Display;


        // 初始化日志系统
        public static void Initialize()
        {
            _logWriter = new LogWriter();

            // 加载LogSettings资源文件
            string[] guids = AssetDatabase.FindAssets("t:LogSettings");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                LogSettings settings = AssetDatabase.LoadAssetAtPath<LogSettings>(path);
                if (settings != null)
                {
                    LogOutputLevel = settings.logOutputLevel;
                }
            }
        }

        // 写入日志
        // ReSharper disable once MemberCanBePrivate.Global
        public static void Log(LogLevel level, string message, params object[] args)
        {
            // 只有当日志级别高于指定的输出级别时才进行输出
            if (level > LogOutputLevel)
            {
                return;
            }

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

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void OnInitializeOnLoad()
        {
            UnityEditor.BuildPlayerWindow.RegisterBuildPlayerHandler(OnBuildPlayer);
        }

        private static void OnBuildPlayer(BuildPlayerOptions options)
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:LogSettings");
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                options.assetBundleManifestPath = path;
            }
        }
#endif
    }

    public class FatalException : Exception
    {
        public FatalException(string message) : base(message)
        {
        }
    }
}