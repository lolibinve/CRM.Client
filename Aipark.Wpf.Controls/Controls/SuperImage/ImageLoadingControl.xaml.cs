using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Aipark.Wpf.Controls.SuperImage
{
    /// <summary>
    /// ImageLoadingControl.xaml 的交互逻辑
    /// </summary>
    public partial class ImageLoadingControl : UserControl
    {
        private readonly Storyboard _storyBoard = null;

        /// <summary>
        /// 点击刷新事件
        /// </summary>
        public event Action Click;

        public ImageLoadingControl()
        {
            InitializeComponent();

            _storyBoard = this.FindResource("ProgressStoryboard") as Storyboard;
        }

        /// <summary>
        /// 图像加载状态
        /// </summary>
        public ImageLoadingStatus Status
        {
            get { return (ImageLoadingStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(nameof(Status), typeof(ImageLoadingStatus), typeof(ImageLoadingControl), new PropertyMetadata(ImageLoadingStatus.Waiting, OnStatusPropertyChanged));
        public static void OnStatusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageLoadingControl control)
            {
                control.image.Visibility = control.Status == ImageLoadingStatus.Loading ? Visibility.Visible : Visibility.Collapsed;
                control.textBlock.Visibility = control.Status == ImageLoadingStatus.Failed ? Visibility.Visible : Visibility.Collapsed;

                if (control.Status == ImageLoadingStatus.Loading)
                {
                    if (control._storyBoard != null && (ImageLoadingStatus)e.OldValue != ImageLoadingStatus.Loading)
                    {
                        control._storyBoard.Begin();
                    }
                }
                else
                {
                    control._storyBoard?.Stop();
                }
            }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Fault
        {
            get { return (string)GetValue(FaultProperty); }
            set { SetValue(FaultProperty, value); }
        }
        public static readonly DependencyProperty FaultProperty =
            DependencyProperty.Register(nameof(Fault), typeof(string), typeof(ImageLoadingControl), new PropertyMetadata(string.Empty));

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            this.Click?.Invoke();
        }

    }
}
