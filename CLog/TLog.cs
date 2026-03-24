using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLog
{
    /// <summary>
    /// 日志类
    /// </summary>
    public static class TLog
    {
        private static NLog.Logger logger;

        static TLog()
        {
            logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public static void Trace(string message)
        {
            logger.Log(LogLevel.Trace, message);
        }
        public static void Trace(Exception exception)
        {
            logger.Log(LogLevel.Trace, exception);
        }
        public static void Trace(Exception exception, string message)
        {
            logger.Log(LogLevel.Trace, exception, message);
        }

        public static void Debug(string message)
        {
            logger.Log(LogLevel.Debug, message);
        }
        public static void Debug(Exception exception)
        {
            logger.Log(LogLevel.Debug, exception);
        }
        public static void Debug(Exception exception, string message)
        {
            logger.Log(LogLevel.Debug, exception, message);
        }

        public static void Info(string message)
        {
            logger.Log(LogLevel.Info, message);
        }
        public static void Info(Exception exception)
        {
            logger.Log(LogLevel.Info, exception);
        }
        public static void Info(Exception exception, string message)
        {
            logger.Log(LogLevel.Info, exception, message);
        }

        public static void Warn(string message)
        {
            logger.Log(LogLevel.Warn, message);
        }
        public static void Warn(Exception exception)
        {
            logger.Log(LogLevel.Warn, exception);
        }
        public static void Warn(Exception exception, string message)
        {
            logger.Log(LogLevel.Warn, exception, message);
        }

        public static void Error(string message)
        {
            logger.Log(LogLevel.Error, message);
        }
        public static void Error(Exception exception)
        {
            logger.Log(LogLevel.Error, exception);
        }
        public static void Error(Exception exception, string message)
        {
            logger.Log(LogLevel.Error, exception, message);
        }

        public static void Fatal(string message)
        {
            logger.Log(LogLevel.Fatal, message);
        }
        public static void Fatal(Exception exception)
        {
            logger.Log(LogLevel.Fatal, exception);
        }
        public static void Fatal(Exception exception, string message)
        {
            logger.Log(LogLevel.Fatal, exception, message);
        }


        public static void Log(LogLevel level, string message)
        {
            logger.Log(level, message);
        }
        public static void Log(LogLevel level, Exception exception)
        {
            logger.Log(level, exception);
        }
        public static void Log(LogLevel level, Exception exception, string message)
        {
            logger.Log(level, exception, message);
        }

    }
}
