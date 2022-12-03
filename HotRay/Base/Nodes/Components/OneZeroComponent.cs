using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components
{
    public class OneZeroComponent<rayT> : ComponentBase 
        where rayT : RayBase
    {

        public OneZeroComponent() : base()
        {
            inPort0 = CreatePort<rayT>();
        }

        public OneZeroComponent(OneZeroComponent<rayT> other) : base(other)
        {
            inPort0 = CreatePort<rayT>(other.inPort0);
        }


        protected readonly Port<rayT> inPort0;


        public override IPort[] OutputPorts => new IPort[] {};
        public override IPort[] InputPorts => new IPort[] { inPort0 };
    }
}
