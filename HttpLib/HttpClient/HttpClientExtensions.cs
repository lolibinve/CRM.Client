using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpLib.HttpClient
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// 创建<see cref="HttpRequestMessage"/>信息
        /// </summary>
        /// <param name="baseUri">网络基址</param>
        /// <param name="method"><see cref="System.Net.WebRequestMethods.Http"/></param>
        /// <param name="parameters">查询参数</param>
        /// <param name="headers">请求头部</param>
        /// <param name="content">消息体</param>
        /// <param name="stream">流</param>
        /// <returns></returns>
        public static HttpRequestMessage GetHttpRequestMessage(string baseUri, string method, Dictionary<string, string> parameters = null,
            Dictionary<string, string> headers = null, string content = null, Dictionary<string, string> formDataStrDic = null, Dictionary<string, string> formDataFileDic = null)
        {
            string requestUri = baseUri;

            if (parameters != null && parameters.Count > 0)
            {
                requestUri += "?";
                int index = 1;
                foreach (var key in parameters.Keys)
                {
                    string suffix = index++ < parameters.Keys.Count ? "&" : "";
                    requestUri += $"{key}={parameters[key]}{suffix}";
                }
            }

            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), requestUri);

            //默认的请求头(防止某些服务响应异常)
            headers = headers ?? new Dictionary<string, string>();
            headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.20 Safari/537.36 Edg/97.0.1072.13");
            headers.Add("Accept-Language", "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7,mt;q=0.6");

            foreach (var key in headers.Keys)
            {
                if (request.Headers.Contains(key))
                {
                    request.Headers.Remove(key);
                }
                request.Headers.Add(key, headers[key]);
            }

            if (formDataFileDic != null || formDataStrDic!=null)
            {
                var mfdc = new MultipartFormDataContent();
                mfdc.Headers.Add("ContentType", "multipart/form-data");//声明头部

                if (formDataFileDic != null)
                {
                    foreach (var key in formDataFileDic)
                    {

                        var temp = new ByteArrayContent(File.ReadAllBytes(key.Value));
                        //mfdc.Add(temp, key.Key);
                        mfdc.Add(temp, key.Key, key.Value);
                    }
                }
                if (formDataStrDic != null)
                {
                    foreach (var item in formDataStrDic)
                    {
                        if(item.Value != null)
                        {
                            mfdc.Add(new StringContent(item.Value), item.Key);//参数, 内容在前,参数名称在后
                        }
                    }
                }
                request.Content = mfdc;
            }
            else if (!string.IsNullOrWhiteSpace(content))
            {
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }

            return request;
        }

        /// <summary>
        /// 返回HTTP响应消息
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="request"></param>
        /// <param name="cts"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> GetResponseAsync(this System.Net.Http.HttpClient httpClient, HttpRequestMessage request, CancellationTokenSource cts = null)
        {
            if (cts != null)
            {
                return await httpClient.SendAsync(request, cts.Token);
            }
            else
            {
                return await httpClient.SendAsync(request);
            }
        }

        /// <summary>
        /// 获取指定格式的响应信息
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="method">HTTP方法</param>
        /// <param name="responseContentType">响应消息体类型</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="headers">请求头</param>
        /// <param name="content">消息体</param>
        /// <param name="cts">取消令牌</param>
        /// <param name="attempts">重试次数</param>
        /// <returns></returns>
        public static async Task<HttpResult> GetResponseAsync(string url, string method, HttpResponseContentType responseContentType, Dictionary<string, string> parameters = null,
            Dictionary<string, string> headers = null, string content = null, CancellationTokenSource cts = null,
            int attempts = 1, Dictionary<string, string> formDataStrDic = null, Dictionary<string, string> formDataFileDic = null)
        {
            int times = 0;
            attempts = Math.Max(attempts, 1);

            HttpResult httpResult = new HttpResult();
            do
            {
                times++;

                httpResult.Reset();

                try
                {
                    using (HttpRequestMessage request = GetHttpRequestMessage(url, method, parameters, headers, content, formDataStrDic, formDataFileDic))
                    {
                        httpResult.RequestUri = request.RequestUri.OriginalString;

                        using (HttpResponseMessage response = await DependencyInjectionHelper.HttpClient.GetResponseAsync(request, cts))
                        {
                            httpResult.IsSuccess = response.IsSuccessStatusCode;
                            httpResult.StatusCode = response.StatusCode;
                            if (httpResult.IsSuccess)
                            {
                                switch (responseContentType)
                                {
                                    case HttpResponseContentType.String:
                                        httpResult.Content = await response.Content.ReadAsStringAsync();
                                        break;

                                    case HttpResponseContentType.Bytes:
                                        httpResult.Bytes = await response.Content.ReadAsByteArrayAsync();
                                        break;

                                    case HttpResponseContentType.Stream:
                                        using (var stream = await response.Content.ReadAsStreamAsync())
                                        {
                                            MemoryStream memoryStream = new MemoryStream();
                                            await stream.CopyToAsync(memoryStream);

                                            httpResult.Stream = memoryStream;
                                        }
                                        break;

                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                httpResult.Message = await response.Content.ReadAsStringAsync();
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex is TaskCanceledException)
                    {
                        if (cts != null && cts.IsCancellationRequested)
                        {
                            httpResult.Exception = ex;
                            break;
                        }
                    }

                    httpResult.Exception = ex;
                    GC.Collect();
                }

                if (httpResult.IsSuccess == false && times < attempts)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500 * Math.Pow(2, times)));
                }

            } while (httpResult.IsSuccess == false && times < attempts);

            return httpResult;
        }


        #region GET

        public static async Task<HttpResult> GetAsync(string url, Dictionary<string, string> parameters = null,
            Dictionary<string, string> headers = null, CancellationTokenSource cts = null, int attempts = 1, Dictionary<string, string> formDataStrDic = null, Dictionary<string, string> formDataFileDic = null)
        {
            HttpResult httpResult = await GetResponseAsync(url, "Get", HttpResponseContentType.String, parameters, headers, null, cts, attempts, formDataStrDic, formDataFileDic);
            return httpResult;
        }

        public static async Task<HttpResult> GetBytesAsync(string url, Dictionary<string, string> parameters = null,
            Dictionary<string, string> headers = null, CancellationTokenSource cts = null, int attempts = 1, Dictionary<string, string> formDataStrDic = null, Dictionary<string, string> formDataFileDic = null)
        {
            HttpResult httpResult = await GetResponseAsync(url, "Get", HttpResponseContentType.Bytes, parameters, headers, null, cts, attempts, formDataStrDic, formDataFileDic);
            return httpResult;
        }

        public static async Task<HttpResult> GetStreamAsync(string url, Dictionary<string, string> parameters = null,
            Dictionary<string, string> headers = null, CancellationTokenSource cts = null, int attempts = 1, Dictionary<string, string> formDataStrDic = null, Dictionary<string, string> formDataFileDic = null)
        {
            HttpResult httpResult = await GetResponseAsync(url, "Get", HttpResponseContentType.Stream, parameters, headers, null, cts, attempts, formDataStrDic, formDataFileDic);
            return httpResult;
        }

        #endregion

        #region POST

        public static async Task<HttpResult> PostAsync(string url, string content, Dictionary<string, string> parameters = null,
            Dictionary<string, string> headers = null, CancellationTokenSource cts = null, int attempts = 1, Dictionary<string, string> formDataStrDic = null, Dictionary<string, string> formDataFileDic = null)
        {
            HttpResult httpResult = await GetResponseAsync(url, "Post", HttpResponseContentType.String, parameters, headers, content, cts, attempts, formDataStrDic, formDataFileDic);
            return httpResult;
        }

        #endregion

    }


    /// <summary>
    /// HTTP响应体类型
    /// </summary>
    public enum HttpResponseContentType
    {
        /// <summary>
        /// 字符串
        /// </summary>
        String = 0,

        /// <summary>
        /// 字节数组
        /// </summary>
        Bytes = 1,

        /// <summary>
        /// 流
        /// </summary>
        Stream = 2
    }
}
