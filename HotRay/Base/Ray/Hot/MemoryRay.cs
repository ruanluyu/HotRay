using HotRay.Base.Ray.Hot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray
{
    public class MemoryRay<dataT> : HotRayBase<dataT[]>
    {
        public MemoryRay():base()
        {
            Data = null;
        }

        public MemoryRay(MemoryRay<dataT> other):base(other)
        {
            Data = other.Data?.ToArray();
        }

        public virtual void ResizeMemory(ulong bytes)
        {
            Data = new dataT[bytes];
        }

        public virtual bool Empty => (Data?.Length ?? 0) == 0;


        public override RayBase RayClone()
        {
            return new MemoryRay<dataT>(this);
        }
    }
}
