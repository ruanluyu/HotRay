using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray.Hot
{
    public struct Color<colorT>
        where colorT: struct
    {
        public colorT r, g, b, a;
    }

    public class ImageRay<colorT>:TensorRay<Color<colorT>>
        where colorT:struct
    {

        public ImageRay() : base() { }
        public ImageRay(ImageRay<colorT> other) : base(other) { }

        public virtual void SetSize(uint w, uint h)
        {
            _Reshape(w, h);
        }

        public virtual uint Width => Empty ? 0 : Dims[0];
        public virtual uint Height => Empty ? 0 : Dims[1];

        public virtual Color<colorT> Get(uint x, uint y)
        {
            return _Get(x, y);
        }

        public virtual void Set(Color<colorT> c, uint x, uint y)
        {
            _Set(c, x, y);
        }

        public virtual void Fill(Color<colorT> c)
        {
            _Fill(c);
        }

        public override IRay RayClone()
        {
            return new ImageRay<colorT>(this);
        }

        public static ImageRay<byte> LoadRGBA256ImageFromPath(string? path)
        {
            throw new NotImplementedException();
        }

        public bool SaveRGBA256ImageFromPath(string? path, bool forceOverride = false)
        {
            throw new NotImplementedException();
        }
    }
}
