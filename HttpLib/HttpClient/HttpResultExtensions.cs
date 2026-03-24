using CLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NLog;

namespace HttpLib.HttpClient
{
    public static class HttpResultExtensions
    {
        /// <summary>
        /// 记录请求响应信息
        /// </summary>
        /// <param name="httpResult"></param>
        /// <param name="content"></param>
        /// <param name="title"></param>
        public static void LogDebug(this HttpResult httpResult, string content = "", string title = "")
        {
            Log(httpResult, LogLevel.Debug, content, title);
        }

        /// <summary>
        /// 记录请求响应信息
        /// </summary>
        /// <param name="httpResult"></param>
        /// <param name="content"></param>
        /// <param name="title"></param>
        public static void LogInfo(this HttpResult httpResult, string content = "", string title = "")
        {
            Log(httpResult, LogLevel.Info, content, title);
        }

        /// <summary>
        /// 记录请求响应信息
        /// </summary>
        /// <param name="httpResult"></param>
        /// <param name="content"></param>
        /// <param name="title"></param>
        public static void LogWarn(this HttpResult httpResult, string content = "", string title = "")
        {
            Log(httpResult, LogLevel.Warn, content, title);
        }

        /// <summary>
        /// 记录请求响应信息
        /// </summary>
        /// <param name="httpResult"></param>
        /// <param name="content"></param>
        /// <param name="title"></param>
        public static void LogError(this HttpResult httpResult, string content = "", string title = "")
        {
            Log(httpResult, LogLevel.Error, content, title);
        }

        /// <summary>
        /// 记录请求响应信息
        /// </summary>
        /// <param name="httpResult"></param>
        /// <param name="level"></param>
        /// <param name="content"></param>
        /// <param name="title"></param>
        public static void Log(this HttpResult httpResult, LogLevel level, string content = "", string title = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                {
                    switch (level.Name)
                    {
                        case "Trace":
                        case "Debug":
                        case "Info":
                            title = $"HTTP日志";
                            break;

                        case "Warn":
                            title = $"HTTP警告";
                            break;

                        case "Error":
                        case "Fatal":
                            title = $"HTTP异常";
                            break;

                        default:
                            title = $"HTTP日志";
                            break;
                    }
                }

                string message = $"{title}：{httpResult.RequestUri}";

                if (httpResult.Exception == null)
                {
                    message = $"{message}{Environment.NewLine} 状态码：{httpResult.StatusCode} ({(int)httpResult.StatusCode})";

                    //请求信息
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        if (content.Length > 4096)
                        {
                            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", DateTime.Now.ToString("yyyy-MM-dd"), "attachments");
                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }
                            string fileName = $"{Guid.NewGuid()}.txt";
                            string filePath = Path.Combine(dir, fileName);
                            File.WriteAllText(filePath, content);

                            message = $"{message}{Environment.NewLine} 请求体：{fileName}";
                        }
                        else
                        {
                            message = $"{message}{Environment.NewLine} 请求体：{content}";
                        }
                    }

                    //响应信息
                    if (string.IsNullOrWhiteSpace(httpResult.Message))
                    {
                        string response = string.Empty;
                        if (!string.IsNullOrWhiteSpace(httpResult.Content))
                        {
                            response = httpResult.Content;
                        }
                        else if (httpResult.Bytes != null)
                        {
                            response = $"byte[]，{httpResult.Bytes.Length}";
                        }
                        else if (httpResult.Stream != null)
                        {
                            response = $"stream，{httpResult.Stream.Length}";
                        }
                        message = $"{message}{Environment.NewLine} 响应体：{response}";
                    }
                    else
                    {
                        message = $"{message}{Environment.NewLine} 响应体：{httpResult.Message}";
                    }

                    TLog.Log(level, message);
                }
                else
                {
                    TLog.Log(level, httpResult.Exception, $"{title}：{httpResult.RequestUri}{Environment.NewLine}");
                }
            }
            catch (Exception ex)
            {
                TLog.Fatal(ex);
            }
        }

        /// <summary>
        /// 主动取消(快速切换等原因)
        /// </summary>
        /// <param name="httpResult"></param>
        /// <param name="cts"></param>
        /// <returns></returns>
        public static bool IsVoluntarilyCancelled(this HttpResult httpResult, CancellationTokenSource cts)
        {
            if (cts != null && cts.IsCancellationRequested && httpResult.Exception != null && httpResult.Exception is TaskCanceledException)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 友好的错误信息
        /// </summary>
        /// <param name="httpResult"></param>
        public static string ErrorMessage(this HttpResult httpResult)
        {
            if (httpResult.Exception != null)
            {
                return httpResult.Exception.GetInnerMessage();
            }
            else
            {
                return httpResult.Message;
            }
        }

    }
}
