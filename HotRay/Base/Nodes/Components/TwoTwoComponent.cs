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
    public class TwoTwoComponent<in0RayT, in1RayT, out0RayT, out1RayT> : ComponentBase 
        where in0RayT : RayBase
        where in1RayT : RayBase
        where out0RayT : RayBase
        where out1RayT : RayBase
    {

        public TwoTwoComponent() : base()
        {

        }

        public TwoTwoComponent(TwoTwoComponent<in0RayT, in1RayT, out0RayT, out1RayT> other) : base(other)
        {

        }


        protected readonly Port<in0RayT> inPort0 = new Port<in0RayT>();
        protected readonly Port<in1RayT> inPort1 = new Port<in1RayT>();
        protected readonly Port<out0RayT> outPort0 = new Port<out0RayT>();
        protected readonly Port<out1RayT> outPort1 = new Port<out1RayT>();


        public override IPort[] OutputPorts => new IPort[] { outPort0, outPort1 };
        public override IPort[] InputPorts => new IPort[] { inPort0, inPort1 };
    }
}
