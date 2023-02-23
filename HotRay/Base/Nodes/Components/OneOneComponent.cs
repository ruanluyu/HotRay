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
            inPort0 = CreateInPort<inRayT>();
            outPort0 = CreateOutPort<outRayT>();
            inPortList = new InPort[] { inPort0 };
            outPortList = new OutPort[] { outPort0 };
        }

        public OneOneComponent(OneOneComponent<inRayT, outRayT> other) : base(other)
        {
            inPort0 = CreatePortFrom<inRayT>(other.inPort0);
            outPort0 = CreatePortFrom<outRayT>(other.outPort0);
            inPortList = new InPort[] { inPort0 };
            outPortList = new OutPort[] { outPort0 };
        }


        protected readonly InPort<inRayT> inPort0;
        protected readonly OutPort<outRayT> outPort0;



    }
}
