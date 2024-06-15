using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LogSystem
{
    public class LogWriter
    {
        private const string LogFileNameFormat = "{0}.log";
        private const string LogBackupFileNameFormat = "{0}-backup-{1}.log";

        private StreamWriter _logFileWriter;

        private readonly StringBuilder _logBuffer = new StringBuilder();
        private const int BufferSize = 1024; // 缓冲区大小,可根据需要调整

        public LogWriter()
        {
            string logFilePath = GetLogFilePath();
            BackupExistingLogFile(logFilePath);
            _logFileWriter = new StreamWriter(logFilePath, true);
        }

        public void WriteLog(LogLevel level, string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss:fff");
            string logEntry = $"[{timestamp}][{GetLogLevelString(level)}] {message}";

            lock (_logBuffer)
            {
                _logBuffer.AppendLine(logEntry);
            }


#if UNITY_EDITOR
            switch (level)
            {
                case LogLevel.Display:
                    Debug.Log(logEntry);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(logEntry);
                    break;
                case LogLevel.Error:
                    Debug.LogError(logEntry);
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

            if (_logBuffer.Length >= BufferSize)
            {
                FlushLogBuffer();
            }
        }
        
        public void Dispose()
        {
            string logContent;

            lock (_logBuffer)
            {
                logContent = _logBuffer.ToString();
                _logBuffer.Clear();
            }
            _logFileWriter.Write(logContent);
            _logFileWriter.Flush();
            _logFileWriter.Close();
        }

        private void FlushLogBuffer()
        {
            string logContent;

            lock (_logBuffer)
            {
                logContent = _logBuffer.ToString();
                _logBuffer.Clear();
            }

#if !UNITY_WEBGL
            // 在支持多线程的平台上使用异步写入
            Task.Run(() =>
            {
                _logFileWriter.Write(logContent);
                _logFileWriter.Flush();
            });
#else
            // 在 WebGL 和编辑器模式下使用同步写入
            _logFileWriter.Write(logContent);
            _logFileWriter.Flush();
#endif
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
            int backupMaxCount = LogSettings.Instance.logFileBackupMaxCount;
            if (File.Exists(logFilePath))
            {
                string backupFileName = string.Format(LogBackupFileNameFormat, Application.productName, DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss"));
                string backupFilePath = Path.Combine(Path.GetDirectoryName(logFilePath) ?? string.Empty, backupFileName);
                File.Copy(logFilePath, backupFilePath, true);
                File.Delete(logFilePath);

                string[] backupFiles = Directory.GetFiles(Path.GetDirectoryName(logFilePath) ?? string.Empty, $"{Application.productName}-backup-*.log");

                Array.Sort(backupFiles, (x, y) => File.GetCreationTime(x).CompareTo(File.GetCreationTime(y)));

                if (backupFiles.Length > backupMaxCount)
                {
                    for (int i = 0; i < backupFiles.Length - backupMaxCount; i++)
                    {
                        File.Delete(backupFiles[i]);
                    }
                }
            }
        }

        private string GetLogLevelString(LogLevel level)
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
    }
}