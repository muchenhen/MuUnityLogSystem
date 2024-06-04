using System;
using System.IO;
using UnityEngine;

namespace LogSystem
{
    public class LogWriter
    {
        private const string LogFileNameFormat = "{0}.log";
        private const string LogBackupFileNameFormat = "{0}-backup-{1}.log";

        private StreamWriter _logFileWriter;

        public LogWriter()
        {
            string logFilePath = GetLogFilePath();
            BackupExistingLogFile(logFilePath);
            _logFileWriter = new StreamWriter(logFilePath, true);
        }

        public void WriteLog(Logger.LogLevel level, string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss:fff");
            string logEntry = $"[{timestamp}][{GetLogLevelString(level)}] {message}";

            // 根据编译符号控制日志输出
#if UNITY_EDITOR
            switch (level)
            {
                case Logger.LogLevel.Display:
                    Debug.Log(logEntry);
                    break;
                case Logger.LogLevel.Warning:
                    Debug.LogWarning(logEntry);
                    break;
                case Logger.LogLevel.Error:
                    Debug.LogError(logEntry);
                    break;
            }
#elif DEVELOPMENT_BUILD
        if (level != Logger.LogLevel.Log)
        {
            UnityEngine.Debug.Log(logEntry);
        }
#endif

         
            _logFileWriter.WriteLine(logEntry);
            _logFileWriter.Flush();
        }

        private string GetLogFilePath()
        {
            string projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
            string logDirectory = Path.Combine(projectRoot, "Saved", "Logs");
            Directory.CreateDirectory(logDirectory);
            string logFileName = string.Format(LogFileNameFormat, Application.productName);
            return Path.Combine(logDirectory, logFileName);
        }

        private void BackupExistingLogFile(string logFilePath)
        {
            if (File.Exists(logFilePath))
            {
                string backupFileName = string.Format(LogBackupFileNameFormat, Application.productName, DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss"));
                string backupFilePath = Path.Combine(Path.GetDirectoryName(logFilePath) ?? string.Empty, backupFileName);
                File.Copy(logFilePath, backupFilePath, true);
                File.Delete(logFilePath);
            }
        }

        private string GetLogLevelString(Logger.LogLevel level)
        {
            switch (level)
            {
                case Logger.LogLevel.Log:
                    return "Log";
                case Logger.LogLevel.Display:
                    return "Display";
                case Logger.LogLevel.Warning:
                    return "Warning";
                case Logger.LogLevel.Error:
                    return "Error";
                default:
                    return "Unknown";
            }
        }
    }
}