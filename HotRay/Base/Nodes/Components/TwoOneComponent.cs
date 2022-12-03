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
    public class TwoOneComponent<in0RayT, in1RayT, outRayT> : ComponentBase 
        where in0RayT : RayBase
        where in1RayT : RayBase
        where outRayT : RayBase
    {
        public TwoOneComponent() : base()
        {
            inPort0 = CreatePort<in0RayT>();
            inPort1 = CreatePort<in1RayT>();
            outPort0 = CreatePort<outRayT>();
        }

        public TwoOneComponent(TwoOneComponent<in0RayT, in1RayT, outRayT> other) : base(other)
        {
            inPort0 = CreatePort<in0RayT>(other.inPort0);
            inPort1 = CreatePort<in1RayT>(other.inPort1);
            outPort0 = CreatePort<outRayT>(other.outPort0);
        }



        protected readonly Port<in0RayT> inPort0 = new Port<in0RayT>();
        protected readonly Port<in1RayT> inPort1 = new Port<in1RayT>();
        protected readonly Port<outRayT> outPort0 = new Port<outRayT>();


        public override IPort[] OutputPorts => new IPort[] { outPort0 };
        public override IPort[] InputPorts => new IPort[] { inPort0, inPort1 };
    }
}
