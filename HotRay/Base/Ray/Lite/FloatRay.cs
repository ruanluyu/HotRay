using HotRay.Base.Ray.Hot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray.Lite
{
    public class FloatRay : LiteRayBase<double>
    {
        public FloatRay() : base() { }
        public FloatRay(FloatRay other) : base(other) { }

        public static implicit operator FloatRay(double other)
        {
            return new FloatRay() { Data = other };
        }

        
        
        public static implicit operator double(FloatRay other)
        {
            return other.Data;
        }
        public override RayBase? CastTo(Type targetType)
        {
            if (targetType == typeof(IntRay)) return new IntRay() { Data = (long)Data };
            if (targetType == typeof(BoolRay)) return new BoolRay() { Data = Data != 0.0 };
            return base.CastTo(targetType);
        }
        public override RayBase RayClone()
        {
            return new FloatRay(this);
        }
        public override string ToString()
        {
            return Data.ToString();
        }
        public override string? ToString(string? format)
        {
            return Data.ToString(format);
        }
    }
}
