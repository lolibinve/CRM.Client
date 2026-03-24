using System.Windows;
using System.Windows.Controls;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// 图像加载状态提示控件
    /// </summary>
    public class ImageProgress : Control
    {
        /// <summary>
        /// 状态
        /// </summary>
        public ImageProgressState State
        {
            get { return (ImageProgressState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(nameof(State), typeof(ImageProgressState), typeof(ImageProgress), new PropertyMetadata(ImageProgressState.Completed));

        /// <summary>
        /// 状态信息
        /// </summary>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(ImageProgress), new FrameworkPropertyMetadata(""));



        static ImageProgress()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageProgress), new FrameworkPropertyMetadata(typeof(ImageProgress)));
        }
    }


    /// <summary>
    /// 图像加载状态
    /// </summary>
    public enum ImageProgressState
    {
        /// <summary>
        /// 加载中
        /// </summary>
        Loading = 0,

        /// <summary>
        /// 加载完成
        /// </summary>
        Completed = 1,

        /// <summary>
        /// 加载失败
        /// </summary>
        Failed = 2
    }
}
