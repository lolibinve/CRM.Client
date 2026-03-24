using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// ImageStateControl.xaml 的交互逻辑
    /// </summary>
    public partial class ImageStateControl : UserControl
    {
        public event Action<ImageStateControl> RefreshButtonClick;

        private readonly Storyboard _storyBoard = null;

        public ImageStateControl()
        {
            InitializeComponent();

            _storyBoard = this.FindResource("ProgressStoryboard") as Storyboard;
        }

        /// <summary>
        /// 设置图像源地址
        /// </summary>
        /// <param name="url"></param>
        public void SetUriSource(string url)
        {
            this.progressContent.Text = string.IsNullOrWhiteSpace(url) ? "" : url;
        }

        /// <summary>
        /// 图像加载状态
        /// </summary>
        public ImageState State
        {
            get { return (ImageState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(nameof(State), typeof(ImageState), typeof(ImageStateControl), new PropertyMetadata(ImageState.Default, StatePropertyChanged));
        public static void StatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageStateControl control)
            {
                control.RefreshProgressPanel();
            }
        }

        private void RefreshProgressPanel()
        {
            switch (this.State)
            {
                case ImageState.Default:
                    this.Visibility = Visibility.Collapsed;
                    this._storyBoard.Stop();
                    break;

                case ImageState.Loading:
                    this.Visibility = Visibility.Visible;
                    this.progressImage.Visibility = Visibility.Visible;
                    this._storyBoard.Begin();
                    this.progressTitle.Visibility = Visibility.Collapsed;
                    this.progressContent.Visibility = Visibility.Collapsed;
                    this.progressButton.Visibility = Visibility.Collapsed;
                    break;

                case ImageState.Loaded:
                    this.Visibility = Visibility.Collapsed;
                    this._storyBoard.Stop();
                    break;

                case ImageState.EmptyUrl:
                    this.Visibility = Visibility.Collapsed;
                    this.progressImage.Visibility = Visibility.Collapsed;
                    this._storyBoard.Stop();
                    this.progressTitle.Visibility = Visibility.Visible;
                    this.progressTitle.Text = "图像地址为空";
                    this.progressContent.Visibility = Visibility.Collapsed;
                    this.progressButton.Visibility = Visibility.Collapsed;
                    break;

                case ImageState.Failed:
                    this.Visibility = Visibility.Visible;
                    this.progressImage.Visibility = Visibility.Collapsed;
                    this._storyBoard.Stop();
                    this.progressTitle.Visibility = Visibility.Visible;
                    this.progressTitle.Text = "图像加载失败";
                    this.progressContent.Visibility = Visibility.Visible;
                    this.progressButton.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshButtonClick?.Invoke(this);
        }
    }
}
