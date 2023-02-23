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
            if(CacheImage)
            {
                _cache = Load();
            }
        }


        ImageRGBA8888Ray? Load()
        {
            if (string.IsNullOrEmpty(ImagePath)) return null;

            ImageRGBA8888Ray? res = null;
            SKBitmap? bitmap = null;

            if (ImagePath.StartsWith("http"))
            {
                var space = GetNearestParent<Space>();
                if(space != null)
                {
                    var httpClient = space.HttpClient;

                    var downloadTask = Task.Run(async () =>
                    {
                        using (Stream stream = await httpClient.GetStreamAsync(ImagePath))
                        using (MemoryStream memStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memStream);
                            memStream.Seek(0, SeekOrigin.Begin);

                            return SKBitmap.Decode(memStream);
                        };
                    });
                    downloadTask.Wait();
                    bitmap = downloadTask.Result;
                }
            }
            else
            {
                if(File.Exists(ImagePath))
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

            if(bitmap != null)
            {
                res = new ImageRGBA8888Ray();
                res.SetSize((uint)bitmap.Width, (uint)bitmap.Height);
                res.Data = bitmap.Pixels.Select(p=> new Color<byte>()
                {
                    r = p.Red,
                    g = p.Green,
                    b = p.Blue,
                    a = p.Alpha
                }).ToArray();
            }

            return res;
        }


        public override NodeBase CloneNode()
        {
            return new SignalToImage(this);
        }

        protected override ImageRGBA8888Ray? ParseRayType(SignalRay? inR)
        {
            if (inR == null) return null;
            if (string.IsNullOrEmpty(ImagePath)) return null;

            if (CacheImage)
            {
                if(_cache == null)
                {
                    _cache = Load();
                    if (_cache == null)
                    {
                        Log($"Failed to load image at {ImagePath}");
                        return null;
                    }
                }
                return new ImageRGBA8888Ray(_cache);
            }
            return Load();
        }
    }
}
