using HotRay.Base.Port;
using HotRay.Base.Ray;
using HotRay.Base.Ray.Lite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components
{
    public abstract class TwoOneComponent<in0RayT, in1RayT, outRayT> : ComponentBase 
        where in0RayT : RayBase
        where in1RayT : RayBase
        where outRayT : RayBase
    {
        public TwoOneComponent() : base()
        {
            inPort0 = CreateInPort<in0RayT>();
            inPort1 = CreateInPort<in1RayT>();
            outPort0 = CreateOutPort<outRayT>();
            inPortList = new InPort[] { inPort0, inPort1 };
            outPortList = new OutPort[] { outPort0 };

        }

        public TwoOneComponent(TwoOneComponent<in0RayT, in1RayT, outRayT> other) : base(other)
        {
            inPort0 = CreatePortFrom<in0RayT>(other.inPort0);
            inPort1 = CreatePortFrom<in1RayT>(other.inPort1);
            outPort0 = CreatePortFrom<outRayT>(other.outPort0);
            inPortList = new InPort[] { inPort0, inPort1 };
            outPortList = new OutPort[] { outPort0 };
        }



        protected readonly InPort<in0RayT> inPort0;
        protected readonly InPort<in1RayT> inPort1;
        protected readonly OutPort<outRayT> outPort0;



    }
}
