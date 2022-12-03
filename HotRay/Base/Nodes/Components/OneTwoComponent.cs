using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components
{
    public class OneTwoComponent<inRayT, out0RayT, out1RayT> : ComponentBase 
        where inRayT : RayBase
        where out0RayT : RayBase
        where out1RayT : RayBase
    {
        public OneTwoComponent() : base()
        {

        }

        public OneTwoComponent(OneTwoComponent<inRayT, out0RayT, out1RayT> other) : base(other)
        {

        }



        protected readonly Port<inRayT> inPort0 = new Port<inRayT>();
        protected readonly Port<out0RayT> outPort0 = new Port<out0RayT>();
        protected readonly Port<out1RayT> outPort1 = new Port<out1RayT>();


        public override IPort[] OutputPorts => new IPort[] { outPort0, outPort1 };
        public override IPort[] InputPorts => new IPort[] { inPort0 };
    }
}
