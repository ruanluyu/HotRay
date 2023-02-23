using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Sources
{
    public abstract class OneRaySource<rayT>:SourceBase 
        where rayT: RayBase
    {
        public OneRaySource() : base()
        {
            outPort0 = CreateOutPort<rayT>();
            outPortList = new OutPort[] {outPort0 };
        }

        public OneRaySource(OneRaySource<rayT> other) : base(other)
        {
            outPort0 = CreatePortFrom<rayT>(other.outPort0);
            outPortList = new OutPort[] { outPort0 };
        }


        protected readonly OutPort<rayT> outPort0;

    }
}
