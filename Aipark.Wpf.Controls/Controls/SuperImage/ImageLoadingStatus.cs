namespace Aipark.Wpf.Controls.SuperImage
{
    /// <summary>
    /// 图像加载状态
    /// </summary>
    public enum ImageLoadingStatus
    {
        /// <summary>
        /// 等待执行
        /// </summary>
        Waiting = 0,

        /// <summary>
        /// 图像正在加载
        /// </summary>
        Loading = 1,

        /// <summary>
        /// 图像加载完成
        /// </summary>
        Loaded = 2,

        /// <summary>
        /// 图像加载失败
        /// </summary>
        Failed = 3
    }
}
