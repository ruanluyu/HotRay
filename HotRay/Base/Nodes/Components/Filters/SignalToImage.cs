using HotRay.Base.Ray.Lite;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotRay.Base.Ray.Hot;

namespace HotRay.Base.Nodes.Components.Filters
{
    using ImageRGBA256Ray = ImageRay<byte>;
    internal class SignalToImage : FilterBase<SignalRay, ImageRGBA256Ray>
    {
        public SignalToImage() : base()
        {

        }

        public SignalToImage(SignalToImage other) : base(other)
        {
            ImagePath = other.ImagePath;
        }


        

        public override void Init()
        {
            base.Init();
            _cache = null;
            if(CacheImage)
            {
                _cache = Load();
            }
        }


        public bool CacheImage { get; set; }
        private ImageRGBA256Ray? _cache;
        public string? ImagePath { get; set; }

        ImageRGBA256Ray? Load()
        {
            if (ImagePath == null) return null;
            return ImageRGBA256Ray.LoadRGBA256ImageFromPath(ImagePath);
        }




        public override INode CloneNode()
        {
            return new SignalToImage(this);
        }

        protected override ImageRGBA256Ray? ParseRayType(SignalRay? inR)
        {
            if (inR == null) return null;
            if (ImagePath == null) return null;

            if (CacheImage)
            {
                if(_cache == null)
                {
                    _cache = Load();
                    if (_cache == null)
                    {
                        Log($"Failed on load image at {ImagePath}");
                        return null;
                    }
                }
                return new ImageRGBA256Ray(_cache);
            }
            return Load();
        }
    }
}
