namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// 图像加载状态
    /// </summary>
    public enum ImageState
    {
        /// <summary>
        /// 
        /// </summary>
        Default = 0,

        /// <summary>
        /// 图像正在加载
        /// </summary>
        Loading = 1,

        /// <summary>
        /// 图像加载完成
        /// </summary>
        Loaded = 2,

        /// <summary>
        /// 图像地址为空
        /// </summary>
        EmptyUrl = 3,

        /// <summary>
        /// 图像加载失败
        /// </summary>
        Failed = 4
    }
}
