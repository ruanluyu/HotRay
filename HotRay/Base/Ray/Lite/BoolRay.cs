using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray.Lite
{
    public class BoolRay : LiteRayBase<bool>
    {
        public BoolRay() : base() { }
        public BoolRay(BoolRay other) : base(other) { }

        public static implicit operator BoolRay(bool other)
        {
            return new BoolRay() { Data = other };
        }

        public static implicit operator bool(BoolRay other)
        {
            return other.Data;
        }

        public override RayBase? CastTo(Type targetType)
        {
            if (targetType == typeof(IntRay)) return new IntRay() { Data = Data ? 1 : 0 };
            if (targetType == typeof(FloatRay)) return new FloatRay() { Data = Data ? 1.0 : 0.0 };
            return base.CastTo(targetType);
        }

        public override RayBase RayClone()
        {
            return new BoolRay(this);
        }

        public override string ToString()
        {
            return Data.ToString();
        }

    }
}
