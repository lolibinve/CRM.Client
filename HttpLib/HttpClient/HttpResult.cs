using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace HttpLib.HttpClient
{
    /// <summary>
    /// Http请求操作结果类
    /// </summary>
    public class HttpResult
    {
        public HttpResult()
        {
            IsSuccess = false;
            StatusCode = HttpStatusCode.BadRequest;
            RequestUri = string.Empty;
            Content = string.Empty;
            Bytes = null;
            Stream = null;
            Message = string.Empty;
            Exception = null;
        }

        public void Reset()
        {
            IsSuccess = false;
            StatusCode = HttpStatusCode.BadRequest;
            RequestUri = string.Empty;
            Content = string.Empty;
            Bytes = null;
            Stream = null;
            Message = string.Empty;
            Exception = null;
        }

        /// <summary>
        /// 该值指示 HTTP 响应是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// HTTP 响应的状态代码
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string RequestUri { get; set; }

        /// <summary>
        /// 请求结果
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 请求结果
        /// </summary>
        public byte[] Bytes { get; set; }

        /// <summary>
        /// 请求结果
        /// </summary>
        public MemoryStream Stream { get; set; }

        public Stream StreamFile { get; set; }

        /// <summary>
        /// 提示信息(来自站点)
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 网络异常
        /// </summary>
        public Exception Exception { get; set; }

        public static implicit operator HttpResult(Stream v)
        {
            throw new NotImplementedException();
        }
    }
}
