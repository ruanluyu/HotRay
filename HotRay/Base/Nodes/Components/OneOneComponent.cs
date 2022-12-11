using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components
{
    public abstract class OneOneComponent<inRayT, outRayT> : ComponentBase 
        where inRayT : RayBase
        where outRayT : RayBase
    {
        public OneOneComponent() : base()
        {
            inPort0 = CreatePort<inRayT>();
            outPort0 = CreatePort<outRayT>();
        }

        public OneOneComponent(OneOneComponent<inRayT, outRayT> other) : base(other)
        {
            inPort0 = CreatePortFrom<inRayT>(other.inPort0);
            outPort0 = CreatePortFrom<outRayT>(other.outPort0);
        }


        protected readonly Port<inRayT> inPort0;
        protected readonly Port<outRayT> outPort0;


        public override IReadOnlyList<IPort> InPorts => new IPort[] { inPort0 };
        public override IReadOnlyList<IPort> OutPorts => new IPort[] { outPort0 };


    }
}
