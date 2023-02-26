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


        public static explicit operator FloatRay(BoolRay other)
        {
            return new FloatRay() { Data = other.Data ? 1.0 : 0.0 };
        }


        
        
        public static implicit operator double(FloatRay other)
        {
            return other.Data;
        }

        public static explicit operator FloatRay(IntRay other)
        {
            return new FloatRay() { Data = other.Data };
        }

        public override RayBase RayClone()
        {
            return new FloatRay(this);
        }
        public override string ToString()
        {
            return Data.ToString();
        }
    }
}
