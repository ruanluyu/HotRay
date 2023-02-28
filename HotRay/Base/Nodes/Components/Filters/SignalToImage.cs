using HotRay.Base.Ray.Lite;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotRay.Base.Ray.Hot;
using SkiaSharp;
using System.Net.Http;
using HotRay.Base.Nodes.Components.Containers;

namespace HotRay.Base.Nodes.Components.Filters
{
    using ImageRGBA8888Ray = ImageRay<byte>;
    internal class SignalToImage : FilterBase<SignalRay, ImageRGBA8888Ray>
    {
        public bool CacheImage { get; set; }
        public string? ImagePath { get; set; }


        public SignalToImage() : base()
        {
            CacheImage = true;
            ImagePath = null;
            _cache = null;
        }

        public SignalToImage(SignalToImage other) : base(other)
        {
            CacheImage = other.CacheImage;
            ImagePath = other.ImagePath;
            _cache = null;
        }

        private ImageRGBA8888Ray? _cache;


        public override void Init()
        {
            base.Init();
            _cache = null;
        }


        async Task<ImageRGBA8888Ray?> LoadAsync()
        {
            if (string.IsNullOrEmpty(ImagePath)) return null;

            ImageRGBA8888Ray? res = null;
            SKBitmap? bitmap = null;

            if (ImagePath.StartsWith("http"))
            {
                var httpClient = HttpClient;

                using (Stream stream = await httpClient.GetStreamAsync(ImagePath))
                using (MemoryStream memStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memStream);
                    memStream.Seek(0, SeekOrigin.Begin);
                    bitmap = SKBitmap.Decode(memStream);
                }
            }
            else
            {
                if (File.Exists(ImagePath))
                {
                    try
                    {
                        var localimage = SKBitmap.Decode(ImagePath);
                        bitmap = localimage;
                    }
                    catch (Exception e)
                    {
                        Log($"Error: {e}");
                        throw;
                    }
                }
            }

            if (bitmap != null)
            {
                res = new ImageRGBA8888Ray();
                res.Set(bitmap.Width, bitmap.Height, 4,
                    bitmap.Pixels.SelectMany(p => new byte[]
                {
                    p.Red,
                    p.Green,
                    p.Blue,
                    p.Alpha
                }).ToArray());
            }

            return res;
        }



        public override NodeBase CloneNode()
        {
            return new SignalToImage(this);
        }

        protected override async Task<ImageRGBA8888Ray?> ParseRayType(SignalRay? inR)
        {
            if (inR == null) return null;
            if (string.IsNullOrEmpty(ImagePath)) return null;

            if (CacheImage)
            {
                if(_cache == null)
                {
                    _cache = await LoadAsync();
                    if (_cache == null)
                    {
                        Log($"Failed to load image at {ImagePath}");
                        return null;
                    }
                }
                return new ImageRGBA8888Ray(_cache);
            }
            return await LoadAsync();
        }
    }
}
