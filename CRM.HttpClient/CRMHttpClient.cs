using HttpLib.HttpClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using CLog;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices.ComTypes;

namespace HttpLib
{
    public  class CRMHttpClient
    {
        /// <summary>
        /// 获取文本格式的响应信息
        /// </summary>
        /// <param name="relativeUri">URL地址</param>
        /// <param name="method">方法</param>
        /// <param name="parameters">查询字符串参数</param>
        /// <param name="headers">HTTP请求头部</param>
        /// <param name="content">消息体</param>
        /// <returns></returns>
        private static async Task<HttpResult> GetTextResponseAsync(string relativeUri, string method,
            Dictionary<string, string> parameters = null, Dictionary<string, string> headers = null, string content = null,
            Dictionary<string, string> formDataStrDic = null, Dictionary<string, string> formDataFileDic = null, CancellationTokenSource cts = null,bool isStream = false)
        {
#if DEBUG
            const string BaseUrl = "http://192.168.1.7:8080";
#else
            const string BaseUrl = "http://175.24.61.38:8080";
#endif
            string absoluteUri = new Uri(BaseUrl).AbsoluteUri + relativeUri.TrimStart('/');

            HttpResult httpResult = new HttpResult();

            try
            {
                using (HttpRequestMessage request = 
                    HttpClientExtensions.GetHttpRequestMessage(absoluteUri, method, parameters, headers, content, formDataStrDic, formDataFileDic))
                {
                    httpResult.RequestUri = request.RequestUri.OriginalString;

                    using (HttpResponseMessage response = await DependencyInjectionHelper.HttpClient.GetResponseAsync(request, cts))
                    {
                        httpResult.IsSuccess = response.IsSuccessStatusCode;
                        httpResult.StatusCode = response.StatusCode;
                        if (httpResult.IsSuccess)
                        {
                            if(isStream)
                            {
                                var stream = await response.Content.ReadAsStreamAsync();
                                httpResult.Bytes = new byte[stream.Length];
                                stream.Read(httpResult.Bytes, 0, httpResult.Bytes.Length);
                            }
                            else
                            {
                                httpResult.Content = await response.Content.ReadAsStringAsync();
                            }

                            CRMHttpResponse acmpHttpResponse = JsonHelper.DeserializeObject<CRMHttpResponse>(httpResult.Content);
                            if (acmpHttpResponse != null && acmpHttpResponse.State != 0)
                            {
                                //日志
                                httpResult.LogError(content);
                            }
                        }
                        else
                        {
                            httpResult.Message = await response.Content.ReadAsStringAsync();
                            httpResult.LogError(content);
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
                        TLog.Error($"HTTP异常：令牌取消，{httpResult.RequestUri}");
                        httpResult.Exception = ex;
                    }
                }

                if (httpResult.Exception == null)
                {
                    httpResult.Exception = ex;
                    httpResult.LogError(content);
                }

                GC.Collect();
            }

            return httpResult;
        }






        #region PUT

        public static async Task<HttpResult> PutAsync(string url,
            Dictionary<string, string> parameters = null, Dictionary<string, string> headers = null,
            Dictionary<string, string> formDataStrDic = null, Dictionary<string, string> formDataFileDic = null, CancellationTokenSource cts = null)
        {
            HttpResult httpResult = await GetTextResponseAsync(url, "Put", parameters, headers, null, formDataStrDic, formDataFileDic, cts);
            return httpResult;
        }

        #endregion

        #region GET

        public static async Task<HttpResult> GetAsync(string url,
            Dictionary<string, string> parameters = null, Dictionary<string, string> headers = null, 
            Dictionary<string, string> formDataStrDic = null, Dictionary<string, string> formDataFileDic = null, CancellationTokenSource cts = null,bool isStream = false)
        {
            HttpResult httpResult = await GetTextResponseAsync(url, "Get", parameters, headers, null, formDataStrDic, formDataFileDic, cts, isStream);
            return httpResult;
        }

        #endregion

        #region POST

        public static async Task<HttpResult> PostAsync(string url, string content,
            Dictionary<string, string> parameters = null, Dictionary<string, string> headers = null, 
            Dictionary<string, string> formDataStrDic = null, Dictionary<string, string> formDataFileDic = null, CancellationTokenSource cts = null)
        {
            HttpResult httpResult = await GetTextResponseAsync(url, "Post", parameters, headers, content, formDataStrDic, formDataFileDic, cts);
            return httpResult;
        }

        #endregion
    }
}
