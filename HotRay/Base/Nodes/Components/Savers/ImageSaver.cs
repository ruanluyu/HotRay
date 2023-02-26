using HotRay.Base.Ray.Hot;
using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Savers
{
    using ImageRGBA8888Ray = ImageRay<byte>;
    public class ImageSaver : SaverBase<ImageRGBA8888Ray>
    {
        public ImageSaver() : base()
        {
            EncodeFormat = Format.PNG;
            Quality = 100;
        }

        public ImageSaver(ImageSaver other) : base(other)
        {
            EncodeFormat = other.EncodeFormat;
            Quality = other.Quality;
        }

        public enum Format
        {
            JPEG,
            PNG,
            Webp
        }

        public Format EncodeFormat
        {
            set;get;
        }

        /// <summary>
        /// 0~100, 
        /// 100: best, 
        /// 0: worst.
        /// </summary>
        public int Quality
        {
            set;get;
        }

        protected override bool OnSave(ImageRGBA8888Ray ray)
        {
            bool suc = false;
            SKBitmap bitmap = new SKBitmap(ray.Width, ray.Height);
            var srcPixels = ray.Data;
            if (srcPixels != null)
            {
                var outColors = new SKColor[ray.Width * ray.Height];
                for (int y = 0; y < ray.Height; y++)
                {
                    for (int x = 0; x < ray.Width; x++)
                    {
                        long pid = (y * ray.Width + (long)x);
                        long id = pid << 2;
                        outColors[pid] = new SKColor(
                            srcPixels[id + 0],
                            srcPixels[id + 1],
                            srcPixels[id + 2],
                            srcPixels[id + 3]
                            );
                    }
                }
                bitmap.Pixels = outColors;


                var path = FilePath;
                var lowerpath = path.ToLower();

                byte[]? data = null;
                using (MemoryStream memStream = new MemoryStream())
                using (SKManagedWStream wstream = new SKManagedWStream(memStream))
                {
                    switch (EncodeFormat)
                    {
                        case Format.JPEG:
                            bitmap.Encode(wstream, SKEncodedImageFormat.Jpeg, Quality);
                            if (!lowerpath.EndsWith(".jpg") && !lowerpath.EndsWith(".jpeg"))
                                path = $"{path}.jpg";
                            break;
                        case Format.PNG:
                            bitmap.Encode(wstream, SKEncodedImageFormat.Png, Quality);
                            if (!lowerpath.EndsWith(".png"))
                                path = $"{path}.png";
                            break;
                        case Format.Webp:
                            bitmap.Encode(wstream, SKEncodedImageFormat.Webp, Quality);
                            if (!lowerpath.EndsWith(".webp"))
                                path = $"{path}.webp";
                            break;
                        default:
                            break;
                    }
                    data = memStream.ToArray();
                }
                if (data != null)
                {
                    try
                    {
                        using (var fs = new FileStream(path, Overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write))
                        {
                            fs.Write(data, 0, data.Length);
                            suc = true;
                        }
                    }
                    catch (IOException e)
                    {
                        Log($"IO Error: {e}");
                        throw;
                    }
                    
                }
            }
            return suc;
        }


        public override NodeBase CloneNode()
        {
            return new ImageSaver(this);
        }
    }
}
