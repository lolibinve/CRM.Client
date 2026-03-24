using CLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Encoder = System.Drawing.Imaging.Encoder;

namespace CRM.Modular.Help
{
    public  class Base64ToImageExtensions
    {
        public static string ImageToBase64(Image image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, image.RawFormat);
                byte[] imageBytes = memoryStream.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        public static string ImageToBase64(string filePath)
        {
            var bys = File.ReadAllBytes(filePath);
            var result = Convert.ToBase64String(bys);
            return result;
        }

        public static string ImageToBase641(string filePath)
        {
            try
            {
                Bitmap bmp = new Bitmap(filePath);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                TLog.Error(ex);
                return null;
            }
        }



        public static Image Base64ToImage(string base64)
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                ms.Write(imageBytes, 0, imageBytes.Length);
                return Image.FromStream(ms, true);
            }
        }

        public static Bitmap Base64ToBitmap(string base64)
        {
            base64 = base64.Replace("data:image/png;base64,", "").Replace("data:image/jgp;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "");//将base64头部信息替换
            byte[] bytes = Convert.FromBase64String(base64);
            MemoryStream memStream = new MemoryStream(bytes);
            Image mImage = Image.FromStream(memStream);
            Bitmap bp = new Bitmap(mImage);
            return bp;
        }

        public static BitmapImage Base64ToBitmapImage(string base64)
        {

            try
            {
                byte[] streamBase = Convert.FromBase64String(base64);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = new MemoryStream(streamBase);
                bi.EndInit();
                return bi;
            }
            catch (Exception ex)
            {
                TLog.Error(ex);
            }
            return null;
        }

        #region 图片压缩并转为base64


        public static string ImageCompressToBase64(int width, int height, string filePath)
        {

            return CompressedPicture(50, filePath);
        }

        /// <summary>
        /// 压缩图片并且转base64
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="filePath">图片地址</param>
        /// <returns></returns>
        public static string CutImage(int width, int height, string filePath)
        {
            var aa = File.ReadAllBytes(filePath);
            MemoryStream msSource = new MemoryStream(aa);
            Bitmap btImage = new Bitmap(msSource);
            msSource.Close();
            Image serverImage = btImage;
            //画板大小
            int finalWidth = width, finalHeight = height;
            int srcImageWidth = serverImage.Width;
            int srcImageHeight = serverImage.Height;
            if (srcImageWidth > srcImageHeight)
            {
                finalHeight = srcImageHeight * width / srcImageWidth;
            }
            else
            {
                finalWidth = srcImageWidth * height / srcImageHeight;
            }
            //新建一个bmp图片
            Image newImage = new Bitmap(width, height);
            //新建一个画板
            Graphics g = Graphics.FromImage(newImage);
            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充
            g.Clear(Color.White);
            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(serverImage, new Rectangle((width - finalWidth) / 2, (height - finalHeight) / 2, finalWidth, finalHeight), 0, 0, srcImageWidth, srcImageHeight, GraphicsUnit.Pixel);
            //以jpg格式保存缩略图
            MemoryStream msSaveImage = new MemoryStream();
            newImage.Save(msSaveImage, ImageFormat.Jpeg);
            serverImage.Dispose();
            newImage.Dispose();
            g.Dispose();
            byte[] imageBytes = msSaveImage.ToArray();
            msSaveImage.Close();
            //return "data:image/jpeg;base64," + Convert.ToBase64String(imageBytes);
            var temp = Convert.ToBase64String(imageBytes);

            ////base64压缩方式一
            //MemoryStream ms = new MemoryStream();
            //byte[] compressedData = (byte[])msSaveImage.ToArray();
            //GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
            //compressedzipStream.Write(imageBytes, 0, imageBytes.Length);
            //compressedzipStream.Close();
            //var data= ms.ToArray();
            //return "data:image/jpeg;base64," + Convert.ToBase64String(data, 0, data.Length);

            ////base64压缩方式二
            //MemoryStream ms = new MemoryStream();
            //Stream s = new BZip2OutputStream(ms);
            //s.Write(imageBytes, 0, imageBytes.Length);
            //s.Close();
            //byte[] compressedData = (byte[])ms.ToArray();
            //return "data:image/jpeg;base64," + Convert.ToBase64String(compressedData, 0, compressedData.Length);
            //return Convert.ToBase64String(imageBytes);
            //return Compress2(Convert.ToBase64String(imageBytes));
            return Compress2(imageBytes);
        }
        public static string Compress2(byte[] rawData)
        {
            //byte[] rawData = System.Text.Encoding.UTF8.GetBytes(rawString.ToString());
            MemoryStream ms = new MemoryStream();
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
            compressedzipStream.Write(rawData, 0, rawData.Length);
            compressedzipStream.Close();
            byte[] zippedData = ms.ToArray();
            return Convert.ToBase64String(zippedData);
        }


        /// <summary>
        /// 高质量压缩图片
        /// </summary>
        /// <param name="flag">压缩质量(数字越小压缩率越高) 1-100</param>
        /// <param name="filePath">图片地址</param>
        /// <returns></returns>
        public static string CompressedPicture(int flag, string filePath)
        {
            var imageByte = File.ReadAllBytes(filePath);
            Stream msSource = new MemoryStream(imageByte);
            var list = new List<StreamContent>();
            var aa = new StreamContent(msSource);
            var image = Image.FromStream(msSource);
            ImageFormat tFormat = image.RawFormat;
            int dHeight = image.Width < 120 ? image.Height : 120;//图片质量低不压缩
            int dWidth = image.Width < 120 ? image.Width : 120;//图片质量低不压缩
            var cWidth = 0;
            var cHeigth = 0;

            //if(image.Width < 400 && image.Width < 400)
            //{
            //    var result = ImageToBase64(filePath);
            //    //释放资源
            //    image.Dispose();
            //    msSource.Dispose();

            //    return result;
            //}

            #region 按比例缩放

            if (image.Width > dHeight || image.Width > dWidth)
            {
                if ((image.Width * dHeight) > (image.Width * dWidth))
                {
                    cWidth = dWidth;
                    cHeigth = (dWidth * image.Height) / (image.Width);
                }
                else
                {
                    cHeigth = dHeight;
                    cWidth = (image.Width * dHeight) / (image.Height);
                }
            }
            else
            {
                cWidth = image.Width;
                cHeigth = image.Height;
            }
            var ob = new Bitmap(dWidth, dHeight);
            var g = Graphics.FromImage(ob);
            //清除画布并使用指定的背景颜色对其进行绘制
            g.Clear(Color.WhiteSmoke);
            //设置合成质量(此处高质量)
            g.CompositingQuality = CompositingQuality.HighQuality;
            //设置平滑模式（指定直线、 曲线和已填充区域的边缘是否使用平滑处理。也称为抗锯齿）
            g.SmoothingMode = SmoothingMode.HighQuality;
            //
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(image, new Rectangle((dWidth - cWidth) / 2, (dHeight - cHeigth) / 2, cWidth, cHeigth), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
            g.Dispose();
            #endregion

            #region 保存图片 设置压缩质量

            var ep = new EncoderParameters();
            var qy = new long[1];
            qy[0] = flag;
            var eParam = new EncoderParameter(Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                var arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (var i = 0; i < arrayICI.Length; i++)
                {
                    if (arrayICI[i].FormatDescription.Equals(".JEPG"))
                    {
                        jpegICIinfo = arrayICI[i];
                        break;
                    }
                }
                MemoryStream msSaveImage = new MemoryStream();
                if (jpegICIinfo != null)
                {
                    ob.Save(msSaveImage, jpegICIinfo, ep);//高质量保存
                }
                else
                {
                    ob.Save(msSaveImage, tFormat);
                }
                byte[] imageBytes = msSaveImage.ToArray();
                msSaveImage.Close();
                //return "data:image/jpeg;base64,"+ Convert.ToBase64String(imageBytes);
                var str = Convert.ToBase64String(imageBytes);
                return str;
            }
            catch
            {
                return "";
            }
            finally
            {
                //释放资源
                image.Dispose();
                ob.Dispose();
            }
            #endregion
        }
        #endregion



    }
}
