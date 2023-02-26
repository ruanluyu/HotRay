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


        public static explicit operator BoolRay(IntRay other)
        {
            return new BoolRay() { Data = other.Data != 0 };
        }

        public static explicit operator BoolRay(FloatRay other)
        {
            return new BoolRay() { Data = other.Data != 0.0 };
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
