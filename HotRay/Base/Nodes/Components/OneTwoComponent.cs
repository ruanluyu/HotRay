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
            inPort0 = CreateInPort<inRayT>();
            outPort0 = CreateOutPort<out0RayT>();
            outPort1 = CreateOutPort<out1RayT>();
            inPortList = new InPort[] { inPort0 };
            outPortList = new OutPort[] { outPort0, outPort1 };
        }

        public OneTwoComponent(OneTwoComponent<inRayT, out0RayT, out1RayT> other) : base(other)
        {
            inPort0 = CreatePortFrom<inRayT>(other.inPort0);
            outPort0 = CreatePortFrom<out0RayT>(other.outPort0);
            outPort1 = CreatePortFrom<out1RayT>(other.outPort1);
            inPortList = new InPort[] { inPort0 };
            outPortList = new OutPort[] { outPort0, outPort1 };
        }



        protected readonly InPort<inRayT> inPort0;
        protected readonly OutPort<out0RayT> outPort0;
        protected readonly OutPort<out1RayT> outPort1;


    }
}
