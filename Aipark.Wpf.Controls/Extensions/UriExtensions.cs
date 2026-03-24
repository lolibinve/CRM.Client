using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;

namespace T.Wpf.Controls.Extensions
{
    /// <summary>
    /// Uri操作扩展类
    /// </summary>
    public static class UriExtensions
    {
        public static async Task<byte[]> GetImageBytesAsync(this Uri uri)
        {
            MemoryStream memoryStream = await uri.GetImageStreamAsync();
            if (memoryStream != null)
            {
                return memoryStream.ToArray();
            }
            return null;
        }

        public static async Task<MemoryStream> GetImageStreamAsync(this Uri uri)
        {
            MemoryStream memoryStream = null;

            if (uri.IsAbsoluteUri)
            {
                if (uri.Scheme.Equals("http", StringComparison.CurrentCultureIgnoreCase) || uri.Scheme.Equals("https", StringComparison.CurrentCultureIgnoreCase))
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        string originalString = uri.OriginalString;
                        if (uri.OriginalString.Contains("oss"))
                        {
                            originalString = originalString.Replace("-internal", "");
                        }

                        using (HttpResponseMessage response = await httpClient.GetAsync(originalString))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                using (var stream = await response.Content.ReadAsStreamAsync())
                                {
                                    memoryStream = new MemoryStream();
                                    stream.CopyTo(memoryStream);
                                    return memoryStream;
                                }
                            }
                            else
                            {
#if DEBUG
                                Console.WriteLine($"{nameof(UriExtensions)}.{nameof(GetImageStreamAsync)}：图像下载失败,{uri.OriginalString},{await response.Content.ReadAsStringAsync()}");
#endif
                            }
                        }
                    }
                }
                else if (uri.Scheme.Equals("file"))
                {
                    using (FileStream fileStream = new FileStream(uri.OriginalString, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        memoryStream = new MemoryStream();
                        fileStream.CopyTo(memoryStream);
                    }
                }
                else if (uri.Scheme.Equals("pack"))
                {
                    StreamResourceInfo streamResourceInfo = Application.GetResourceStream(uri);
                    if (streamResourceInfo != null)
                    {
                        memoryStream = new MemoryStream();
                        streamResourceInfo.Stream.CopyTo(memoryStream);

                        streamResourceInfo.Stream.Dispose();
                    }
                }
                else
                {
#if DEBUG
                    Console.WriteLine($"{nameof(UriExtensions)}.{nameof(GetImageStreamAsync)}：未识别的URI格式,{uri.OriginalString}");
#endif
                }
            }
            else
            {
                StreamResourceInfo streamResourceInfo = Application.GetResourceStream(uri);
                if (streamResourceInfo != null)
                {
                    memoryStream = new MemoryStream();
                    streamResourceInfo.Stream.CopyTo(memoryStream);

                    streamResourceInfo.Stream.Dispose();
                }
            }

            return memoryStream;
        }
    }
}
