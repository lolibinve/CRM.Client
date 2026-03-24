using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Aipark.Wpf.Extensions;
using T.Wpf.Controls.Extensions;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// 提供简单效果的图像控件
    /// </summary>
    public partial class SimpleImage : UserControl
    {
        public SimpleImage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 图像原始分辨率宽度
        /// </summary>
        public int OriginalPixelWidth
        {
            get { return (int)GetValue(OriginalPixelWidthProperty); }
            private set { SetValue(OriginalPixelWidthProperty, value); }
        }
        public static readonly DependencyProperty OriginalPixelWidthProperty =
            DependencyProperty.Register(nameof(OriginalPixelWidth), typeof(int), typeof(ZoomImage), new PropertyMetadata(0));

        /// <summary>
        /// 图像原始分辨率高度
        /// </summary>
        public int OriginalPixelHeight
        {
            get { return (int)GetValue(OriginalPixelHeightProperty); }
            private set { SetValue(OriginalPixelHeightProperty, value); }
        }
        public static readonly DependencyProperty OriginalPixelHeightProperty =
            DependencyProperty.Register(nameof(OriginalPixelHeight), typeof(int), typeof(ZoomImage), new PropertyMetadata(0));

        /// <summary>
        /// 获取或设置图像解码分辨率倍数
        /// </summary>
        public double DecodeFactor
        {
            get { return (double)GetValue(DecodeFactorProperty); }
            set { SetValue(DecodeFactorProperty, value); }
        }
        public static readonly DependencyProperty DecodeFactorProperty =
            DependencyProperty.Register(nameof(DecodeFactor), typeof(double), typeof(ZoomImage), new PropertyMetadata(1.0, DecodeFactorPropertyChanged));
        public static void DecodeFactorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ZoomImage control)
            {
                control.RefreshImage();
            }
        }

        /// <summary>
        /// 图像原始流
        /// </summary>
        public MemoryStream OriginalStream
        {
            get { return (MemoryStream)GetValue(OriginalStreamProperty); }
            private set { SetValue(OriginalStreamProperty, value); }
        }
        public static readonly DependencyProperty OriginalStreamProperty =
            DependencyProperty.Register(nameof(OriginalStream), typeof(MemoryStream), typeof(ZoomImage), new PropertyMetadata(null));

        /// <summary>
        /// 图像源地址(支持网络图像或本地图像)
        /// </summary>
        public string UriSource
        {
            get { return (string)GetValue(UriSourceProperty); }
            set { SetValue(UriSourceProperty, value); }
        }
        public static readonly DependencyProperty UriSourceProperty =
            DependencyProperty.Register("UriSource", typeof(string), typeof(SimpleImage), new PropertyMetadata("!@#$%^&*", OnUriSourcePropertyChanged));
        public static async void OnUriSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SimpleImage control)
            {
                await control.DecodeUriSourceAsync();
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
                        this.layerImage.Source = null;
                        this.OriginalPixelWidth = 0;
                        this.OriginalPixelHeight = 0;
                    }
                    else
                    {
                        using (System.Drawing.Image image = System.Drawing.Image.FromStream(memoryStream))
                        {
                            this.OriginalPixelWidth = image.Width;
                            this.OriginalPixelHeight = image.Height;
                        }

                        //底图渲染
                        this.RefreshImage();
                        this.progressControl.State = ImageState.Loaded;
                    }
                }
                catch (Exception ex)
                {
                    this.progressControl.State = ImageState.Failed;
                    this.layerImage.Source = null;
                    this.OriginalPixelWidth = 0;
                    this.OriginalPixelHeight = 0;
#if DEBUG
                    Console.WriteLine($"图像解析失败({nameof(SimpleImage)})：{uri.OriginalString},{ex.StackTrace}");
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
                this.layerImage.Source = null;
                this.OriginalPixelWidth = 0;
                this.OriginalPixelHeight = 0;
            }
        }

        public void RefreshImage()
        {
            if (this.OriginalStream != null)
            {
                int decodePixelWidth = this.OriginalPixelWidth;
                int decodePixelHeight = this.OriginalPixelHeight;
                if (this.DecodeFactor > 0)
                {
                    decodePixelWidth = (int)(this.OriginalPixelWidth * this.DecodeFactor);
                    decodePixelHeight = (int)(this.OriginalPixelHeight * this.DecodeFactor);
                }

                var bitmapImage = BitmapImageExtensions.FromStream(this.OriginalStream, decodePixelWidth, decodePixelHeight);
                this.layerImage.Source = bitmapImage;
            }
        }

        private async void ProgressControl_RefreshButtonClick(ImageStateControl obj)
        {
            await DecodeUriSourceAsync();
        }
    }
}
