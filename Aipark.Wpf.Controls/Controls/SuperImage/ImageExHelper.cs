using CLog;
using HttpLib;
using HttpLib.HttpClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;

namespace Aipark.Wpf.Controls.SuperImage
{
    public static class ImageExHelper
    {
        /// <summary>
        /// 本地缓存映射字典
        /// </summary>
        private static ConcurrentDictionary<string, string> _localCacheDictionary = new ConcurrentDictionary<string, string>();
        /// <summary>
        /// 锁
        /// </summary>
        private static object o = new object();
        /// <summary>
        /// 下载中的图像地址
        /// </summary>
        private static List<string> _decodingUrls = new List<string>();
        /// <summary>
        /// 图像缓存目录
        /// </summary>
        private static string _tempDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp", "images");

        static ImageExHelper()
        {
            try
            {
                if (Directory.Exists(_tempDirectory))
                {
                    Directory.Delete(_tempDirectory, true);
                }
            }
            catch (Exception ex)
            {
                TLog.Error(ex, "缓存图像清理异常");
            }
        }

        /// <summary>
        /// 解析图像流
        /// </summary>
        /// <param name="request"></param>
        /// <param name="localCache"></param>
        /// <returns></returns>
        public static async Task<ImageDecodeResponse> DecodeImageStreamAsync(ImageDecodeRequest request, bool localCache = false)
        {
            ImageDecodeResponse response = new ImageDecodeResponse() { Url = request.Url, Stream = null, Exception = null };

            try
            {
                Uri uri = new Uri(request.Url, UriKind.RelativeOrAbsolute);
                if (uri.IsAbsoluteUri)
                {
                    if (uri.Scheme.Equals("http") || uri.Scheme.Equals("https"))
                    {
                        if (localCache)
                        {
                            while (_decodingUrls.Contains(request.Url))
                            {
                                await Task.Delay(5);
                            }

                            if (_localCacheDictionary.TryGetValue(request.Url, out string filePath))
                            {
                                if (File.Exists(filePath))
                                {
                                    using (FileStream fileStream = new FileStream(filePath, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read))
                                    {
                                        response.Stream = new MemoryStream();
                                        await fileStream.CopyToAsync(response.Stream);
                                        return response;
                                    }
                                }
                                else
                                {
                                    _localCacheDictionary.TryRemove(request.Url, out _);
                                }
                            }
                        }

                        try
                        {
                            if (localCache)
                            {
                                lock (o)
                                {
                                    _decodingUrls.Add(request.Url);
                                }
                            }

                            Debug.WriteLine($"解析队列长度1：{_decodingUrls.Count}");

                            HttpResult httpResult = await DependencyInjectionHelper.HttpClient.GetStreamAsync(request.Url);
                            if (httpResult.IsSuccess)
                            {
                                if (localCache)
                                {
                                    string directory = Path.Combine(_tempDirectory, DateTime.Now.ToString("yyyyMMdd"));
                                    if (!Directory.Exists(directory))
                                    {
                                        Directory.CreateDirectory(directory);
                                    }

                                    string filePath = Path.Combine(directory, $"{Guid.NewGuid().ToString().Replace("-", "")}.jpg");
                                    File.WriteAllBytes(filePath, httpResult.Stream.ToArray());

                                    _localCacheDictionary.TryAdd(request.Url, filePath);
                                }

                                response.Stream = httpResult.Stream;
                                return response;
                            }
                            else
                            {
                                //资源不存在或网络异常
                                response.Exception = httpResult.Exception ?? new Exception($"network error ({(int)httpResult.StatusCode})");
                                return response;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            if (localCache)
                            {
                                lock (o)
                                {
                                    _decodingUrls.Remove(request.Url);
                                }
                            }
                            Debug.WriteLine($"解析队列长度2：{_decodingUrls.Count}");
                        }
                    }
                    else if (uri.Scheme.Equals("file"))
                    {
                        using (FileStream fileStream = new FileStream(uri.OriginalString, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            response.Stream = new MemoryStream();
                            await fileStream.CopyToAsync(response.Stream);
                            return response;
                        }
                    }
                    else if (uri.Scheme.Equals("pack"))
                    {
                        StreamResourceInfo streamResourceInfo = Application.GetResourceStream(uri);
                        response.Stream = streamResourceInfo.Stream;
                        return response;
                    }
                    else
                    {
                        //不支持的类型
                        response.Exception = new Exception("unsupported uri format");
                        return response;
                    }
                }
                else
                {
                    StreamResourceInfo streamResourceInfo = Application.GetResourceStream(uri);
                    response.Stream = streamResourceInfo.Stream;
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.Exception = ex;
            }

            return response;
        }
    }

    /// <summary>
    /// 图像解析请求
    /// </summary>
    public class ImageDecodeRequest : IDisposable
    {
        /// <summary>
        /// 图像地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 取消令牌
        /// </summary>
        public CancellationTokenSource Cts { get; set; }

        public void Dispose()
        {
            Url = null;
            Cts = null;
        }
    }

    /// <summary>
    /// 图像解析响应
    /// </summary>
    public class ImageDecodeResponse
    {
        /// <summary>
        /// 图像地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 图像流
        /// </summary>
        public Stream Stream { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; set; }
    }
}
