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

        }

        public OneZeroComponent(OneZeroComponent<rayT> other) : base(other)
        {

        }


        protected readonly Port<rayT> inPort0 = new Port<rayT>();


        public override IPort[] OutputPorts => new IPort[] {};
        public override IPort[] InputPorts => new IPort[] { inPort0 };
    }
}
