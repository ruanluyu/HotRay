using HotRay.Base.Ray.Lite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray.Hot
{
    public abstract class HotRayBase<dataT> : RayBase<dataT?>
        where dataT : class
    {
        public HotRayBase() : base() { }
        public HotRayBase(HotRayBase<dataT> other) : base(other) {  }

        public override dataT? Data { get; set; }

        public override string ToString()
        {
            return Data?.ToString() ?? "";
        }

    }
}
