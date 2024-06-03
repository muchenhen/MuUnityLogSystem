namespace LogSystem
{
    public static class Logger
    {
        // 日志等级枚举
        public enum LogLevel
        {
            Log,
            Warning,
            Error
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
        }
    }
}