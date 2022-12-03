using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Sources
{
    public class OneRaySource<rayT>:SourceBase 
        where rayT: RayBase
    {
        public OneRaySource() : base()
        {
            outPort0 = CreatePort<rayT>();
        }

        public OneRaySource(OneRaySource<rayT> other) : base(other)
        {
            outPort0 = CreatePort<rayT>(other.outPort0);
        }


        protected readonly Port<rayT> outPort0;


        public override IPort[] OutputPorts => new IPort[] { outPort0 };


    }
}
