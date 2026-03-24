using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Aipark.Wpf.Extensions
{
    public static class BitmapImageExtensions
    {
        public static BitmapImage FromStream(Stream stream, int decodePixelWidth = 0, int decodePixelHeight = 0)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.InitFromStream(stream, decodePixelWidth, decodePixelHeight);
            return bitmapImage;
        }

        public static void InitFromStream(this BitmapImage bitmapImage, Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
        }

        public static void InitFromStream(this BitmapImage bitmapImage, Stream stream, int decodePixelWidth = 0, int decodePixelHeight = 0)
        {
            stream.Seek(0, SeekOrigin.Begin);

            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = stream;
            bitmapImage.DecodePixelWidth = decodePixelWidth;
            bitmapImage.DecodePixelHeight = decodePixelHeight;
            bitmapImage.EndInit();
        }

        public static void InitFromUri(this BitmapImage bitmapImage, Uri uri, int decodePixelWidth = 0, int decodePixelHeight = 0)
        {
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.UriSource = uri;
            bitmapImage.DecodePixelWidth = decodePixelWidth;
            bitmapImage.DecodePixelHeight = decodePixelHeight;
            bitmapImage.EndInit();
        }
    }
}
