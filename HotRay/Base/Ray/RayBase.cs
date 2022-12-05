using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray
{
    public abstract class RayBase : IRay
    {
        public RayBase(): base() { }

        public RayBase(RayBase other): this() { }


        public abstract IRay RayClone();
    }


    public abstract class RayBase<dataT> : RayBase
    {
        public abstract dataT Data
        {
            get; set;
        }

        public RayBase() : base() { }

        public RayBase(RayBase<dataT> other) : base(other) { }


    }

    
}
