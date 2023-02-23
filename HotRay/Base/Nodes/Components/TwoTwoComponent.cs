using HotRay.Base.Nodes.Sources;
using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components
{
    public abstract class TwoTwoComponent<in0RayT, in1RayT, out0RayT, out1RayT> : ComponentBase 
        where in0RayT : RayBase
        where in1RayT : RayBase
        where out0RayT : RayBase
        where out1RayT : RayBase
    {

        public TwoTwoComponent() : base()
        {
            inPort0 = CreateInPort<in0RayT>();
            inPort1 = CreateInPort<in1RayT>();
            outPort0 = CreateOutPort<out0RayT>();
            outPort1 = CreateOutPort<out1RayT>();
            inPortList = new InPort[] { inPort0, inPort1 };
            outPortList = new OutPort[] { outPort0, outPort1 };
        }

        public TwoTwoComponent(TwoTwoComponent<in0RayT, in1RayT, out0RayT, out1RayT> other) : base(other)
        {
            inPort0 = CreatePortFrom<in0RayT>(other.inPort0);
            inPort1 = CreatePortFrom<in1RayT>(other.inPort1);
            outPort0 = CreatePortFrom<out0RayT>(other.outPort0);
            outPort1 = CreatePortFrom<out1RayT>(other.outPort1);
            inPortList = new InPort[] { inPort0, inPort1 };
            outPortList = new OutPort[] { outPort0, outPort1 };
        }


        protected readonly InPort<in0RayT> inPort0;
        protected readonly InPort<in1RayT> inPort1;
        protected readonly OutPort<out0RayT> outPort0;
        protected readonly OutPort<out1RayT> outPort1;


        

    }
}
