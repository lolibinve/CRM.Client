using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Wpf.Controls.Extensions;

namespace Aipark.Wpf.Controls
{
    public static class ImageCache
    {
        private static readonly ConcurrentDictionary<string, string> imageDic = new ConcurrentDictionary<string, string>();
        private static readonly string dir = "temp\\images";

        static ImageCache()
        {
            try
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine($"ImageCache.ImageCache(),{ex.Message}");
#endif
            }
        }

        public static async Task<MemoryStream> GetMemoryStream(Uri uri)
        {
            MemoryStream ms = null;

            if (imageDic.TryGetValue(uri.AbsoluteUri, out string fileName))
            {
                try
                {
                    var filePath = $"{dir}\\{fileName}";
                    if (File.Exists(filePath))
                    {
                        using (var fs = new FileStream(filePath, System.IO.FileMode.Open))
                        {
                            ms = new MemoryStream();
                            fs.CopyTo(ms);
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine($"ImageCache.GetMemoryStream() 读取文件,{uri.AbsoluteUri},{ex.Message}");
#endif
                }
            }

            if (ms == null)
            {
                ms = await uri.GetImageStreamAsync();
                if (ms != null)
                {
                    try
                    {
                        fileName = Guid.NewGuid().ToString();
                        using (var fs = new FileStream($"{dir}\\{fileName}", System.IO.FileMode.Create))
                        {
                            using (var bw = new BinaryWriter(fs))
                            {
                                bw.Write(ms.ToArray());
                            }
                        }

                        imageDic[uri.AbsoluteUri] = fileName;
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Console.WriteLine($"ImageCache.GetMemoryStream() 写入文件,{uri.AbsoluteUri},{ex.Message}");
#endif
                    }
                }
            }

            return ms;
        }

        public static void Clear()
        {
            try
            {
                imageDic.Clear();

                Directory.Delete(dir, true);

                Directory.CreateDirectory(dir);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine($"ImageCache.Clear(),{ex.Message}");
#endif
            }
        }
    }
}
