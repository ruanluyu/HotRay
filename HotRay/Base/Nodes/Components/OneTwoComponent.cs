using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components
{
    public abstract class OneTwoComponent<inRayT, out0RayT, out1RayT> : ComponentBase 
        where inRayT : RayBase
        where out0RayT : RayBase
        where out1RayT : RayBase
    {
        public OneTwoComponent() : base()
        {
            inPort0 = CreatePort<inRayT>();
            outPort0 = CreatePort<out0RayT>();
            outPort1 = CreatePort<out1RayT>();
        }

        public OneTwoComponent(OneTwoComponent<inRayT, out0RayT, out1RayT> other) : base(other)
        {
            inPort0 = CreatePortFrom<inRayT>(other.inPort0);
            outPort0 = CreatePortFrom<out0RayT>(other.outPort0);
            outPort1 = CreatePortFrom<out1RayT>(other.outPort1);
        }



        protected readonly Port<inRayT> inPort0;
        protected readonly Port<out0RayT> outPort0;
        protected readonly Port<out1RayT> outPort1;


        public override IReadOnlyList<PortBase> OutPorts => new PortBase[] { outPort0, outPort1 };
        public override IReadOnlyList<PortBase> InPorts => new PortBase[] { inPort0 };
    }
}
