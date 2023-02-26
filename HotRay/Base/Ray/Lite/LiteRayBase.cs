using HotRay.Base.Ray.Hot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray.Lite
{
    public abstract class LiteRayBase<dataT> : DataRayBase<dataT>
        where dataT : struct
    {
        public LiteRayBase() : base() { }
        public LiteRayBase(LiteRayBase<dataT> other) : base(other) { Data = other.Data; }

        public override dataT Data { get; set; }

        
        public override RayBase? CastTo(Type targetType)
        {
            if (targetType == typeof(ObjectRay)) return new ObjectRay() { Data = this };
            if (targetType == typeof(StringRay)) return new StringRay() { Data = ToString() };
            return base.CastTo(targetType);
        }

        public override abstract string ToString();

    }
}
