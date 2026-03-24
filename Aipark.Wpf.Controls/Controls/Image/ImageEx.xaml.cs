using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using T.Wpf.Controls.Extensions;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// ImageEx.xaml 的交互逻辑
    /// </summary>
    public partial class ImageEx : UserControl
    {
        public ImageEx()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 缩略图模式
        /// </summary>
        public bool IsThumbnail
        {
            get { return (bool)GetValue(IsThumbnailProperty); }
            set { SetValue(IsThumbnailProperty, value); }
        }
        public static readonly DependencyProperty IsThumbnailProperty =
            DependencyProperty.Register(nameof(IsThumbnail), typeof(bool), typeof(ImageEx), new PropertyMetadata(false));

        /// <summary>
        /// 缩略图模式下的解码宽度
        /// </summary>
        public int ThumbnailPixelWidth
        {
            get { return (int)GetValue(ThumbnailPixelWidthProperty); }
            set { SetValue(ThumbnailPixelWidthProperty, value); }
        }
        public static readonly DependencyProperty ThumbnailPixelWidthProperty =
            DependencyProperty.Register(nameof(ThumbnailPixelWidth), typeof(int), typeof(ImageEx), new PropertyMetadata(480));

        /// <summary>
        /// 图像源
        /// </summary>
        public string UriSource
        {
            get { return (string)GetValue(UriSourceProperty); }
            set { SetValue(UriSourceProperty, value); }
        }
        public static readonly DependencyProperty UriSourceProperty =
            DependencyProperty.Register("UriSource", typeof(string), typeof(ImageEx), new PropertyMetadata("!@#$%^&*", OnUriSourcePropertyChanged));
        public static async void OnUriSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEx view)
            {
                await view.DecodeUriSourceAsync();
            }
        }

        private async Task DecodeUriSourceAsync()
        {
            if (!string.IsNullOrWhiteSpace(this.UriSource))
            {
                this.progressControl.SetUriSource(this.UriSource);
                this.progressControl.State = ImageState.Loading;

                MemoryStream memoryStream = null;

                Uri uri = new Uri(this.UriSource, UriKind.RelativeOrAbsolute);
                try
                {
                    if (uri.IsAbsoluteUri && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                    {
                        memoryStream = await ImageCache.GetMemoryStream(uri);
                    }
                    else
                    {
                        memoryStream = await uri.GetImageStreamAsync();
                    }

                    if (memoryStream == null)
                    {
                        this.progressControl.State = ImageState.Failed;
                    }
                    else
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        if (this.IsThumbnail)
                        {
                            bitmapImage.DecodePixelWidth = this.ThumbnailPixelWidth;
                        }
                        bitmapImage.EndInit();

                        this.Image.Source = bitmapImage;
                        this.progressControl.State = ImageState.Loaded;
                    }
                }
                catch (Exception ex)
                {
                    this.progressControl.State = ImageState.Failed;
                    this.Image.Source = null;
#if DEBUG
                    Console.WriteLine($"图像解析失败({nameof(ImageEx)})：{uri.OriginalString},{ex.StackTrace}");
#endif
                }
                finally
                {
                    memoryStream?.Dispose();
                }
            }
            else
            {
                this.progressControl.State = ImageState.EmptyUrl;
                this.Image.Source = null;
            }
        }

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(ImageEx), new PropertyMetadata(Stretch.Uniform, OnStretchPropertyChanged));
        public static void OnStretchPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageEx view && e.NewValue is Stretch stretch)
            {
                view.Image.Stretch = stretch;
            }
        }

        private async void ProgressControl_RefreshButtonClick(ImageStateControl obj)
        {
            await DecodeUriSourceAsync();
        }
    }
}
