using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components
{
    public class OneOneComponent<inRayT, outRayT> : ComponentBase 
        where inRayT : RayBase
        where outRayT : RayBase
    {
        public OneOneComponent() : base()
        {

        }

        public OneOneComponent(OneOneComponent<inRayT, outRayT> other) : base(other)
        {

        }


        protected readonly Port<inRayT> inPort0 = new Port<inRayT>();
        protected readonly Port<outRayT> outPort0 = new Port<outRayT>();


        public override IPort[] OutputPorts => new IPort[] { outPort0 };
        public override IPort[] InputPorts => new IPort[] { inPort0 };


    }
}
