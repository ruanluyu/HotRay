using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray.Lite
{
    public class IntRay : LiteRayBase<long>
    {
        public IntRay():base() { }
        public IntRay(IntRay other):base(other) { }

        public static explicit operator IntRay(FloatRay floatRay)
        {
            return new IntRay() { Data = (long)floatRay.Data };
        }
        

        public static implicit operator IntRay(long other)
        {
            return new IntRay() { Data = other };
        }


        public static explicit operator IntRay(BoolRay other)
        {
            return new IntRay() { Data = other.Data ? 1 : 0 };
        }


        public static implicit operator long(IntRay other)
        {
            return other.Data;
        }

        public override RayBase RayClone()
        {
            return new IntRay(this);
        }

        public override string ToString()
        {
            return Data.ToString();
        }
        
    }
}
