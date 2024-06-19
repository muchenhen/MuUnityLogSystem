using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

namespace LogSystem
{
    public class LogWriter : IDisposable
    {
        private const string LogFileNameFormat = "{0}.log";
        private const string LogBackupFileNameFormat = "{0}-backup-{1}.log";

        private StreamWriter _logFileWriter;
        private readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
        private readonly object _logQueueLock = new object();
        private readonly Thread _logWriterThread;
        private bool _isRunning = true;

        private readonly BufferedStream _logBufferedStream;


        public LogWriter()
        {
            string logFilePath = GetLogFilePath();
            BackupExistingLogFile(logFilePath);
            _logFileWriter = new StreamWriter(new BufferedStream(File.Open(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read)), Encoding.UTF8);

            _logWriterThread = new Thread(LogWriterThread);
            _logWriterThread.IsBackground = true;
            _logWriterThread.Start();
        }

        public void WriteLog(LogLevel level, string message)
        {
            _logQueue.Enqueue(message);
        }

        public void Dispose()
        {
            _isRunning = false;
            _logWriterThread.Join();
            _logFileWriter.Close();
        }

        private void LogWriterThread()
        {
            while (_isRunning)
            {
                if (_logQueue.TryDequeue(out string logEntry))
                {
                    _logFileWriter.Write(logEntry + "\n");
                    _logFileWriter.Flush();
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
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
    }
}