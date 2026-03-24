using System;
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
        public static IServiceProvider ServiceProvider { get; private set; }

        static DependencyInjectionHelper()
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddHttpClient();

            serviceCollection.AddMemoryCache();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public static T GetService<T>() where T : class
        {
            return ServiceProvider.GetService<T>();
        }

        /// <summary>
        /// 基于依赖注入的，由<see cref="IHttpClientFactory" />管理的<see cref="System.Net.Http.HttpClient" />对象
        /// </summary>
        public static System.Net.Http.HttpClient HttpClient => GetService<IHttpClientFactory>().CreateClient();

        /// <summary>
        /// 基于依赖注入的<see cref="Microsoft.Extensions.Caching.Memory.MemoryCache" />对象
        /// </summary>
        public static IMemoryCache MemoryCache => GetService<IMemoryCache>();

    }
}
