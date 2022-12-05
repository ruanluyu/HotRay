using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray.Lite
{
    public class LiteRayBase<dataT> : RayBase<dataT>
        where dataT : struct
    {
        public LiteRayBase() : base() { }
        public LiteRayBase(LiteRayBase<dataT> other) : base(other) { Data = other.Data; }

        public override dataT Data { get; set; }

        public override IRay RayClone() => new LiteRayBase<dataT>(this);
    }
}
