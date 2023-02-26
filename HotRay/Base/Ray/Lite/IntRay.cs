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

        

        public static implicit operator IntRay(long other)
        {
            return new IntRay() { Data = other };
        }



        public static implicit operator long(IntRay other)
        {
            return other.Data;
        }

        public override RayBase? CastTo(Type targetType)
        {
            if (targetType == typeof(FloatRay)) return new FloatRay() { Data = Data };
            if (targetType == typeof(BoolRay)) return new BoolRay() { Data = Data != 0 };
            return base.CastTo(targetType);
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
