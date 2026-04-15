using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace HttpLib
{
    /// <summary>
    /// 依赖注入辅助类
    /// </summary>
    public static class DependencyInjectionHelper
    {
        private static readonly HttpClientHandler SharedHttpHandler = new HttpClientHandler
        {
            CookieContainer = new CookieContainer(),
            UseCookies = true
        };

        /// <summary>
        /// 登录接口返回的 sessionId，通过请求头 <c>X-Session-Id</c> 传给后端。
        /// </summary>
        public static string CurrentSessionId { get; set; }

        private static readonly System.Net.Http.HttpClient SharedHttpClient =
            new System.Net.Http.HttpClient(SharedHttpHandler, disposeHandler: true);

        public static IServiceProvider ServiceProvider { get; private set; }

        static DependencyInjectionHelper()
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddMemoryCache();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public static T GetService<T>() where T : class
        {
            return ServiceProvider.GetService<T>();
        }

        /// <summary>
        /// 全进程单例；登录后自动携带会话 Cookie。
        /// 会话失效时由应用重启新进程，此处无需清空 Cookie。
        /// </summary>
        public static System.Net.Http.HttpClient HttpClient => SharedHttpClient;

        /// <summary>
        /// 基于依赖注入的<see cref="Microsoft.Extensions.Caching.Memory.MemoryCache" />对象
        /// </summary>
        public static IMemoryCache MemoryCache => GetService<IMemoryCache>();

    }
}
