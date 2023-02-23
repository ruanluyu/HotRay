using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components
{
    public abstract class OneZeroComponent<rayT> : ComponentBase 
        where rayT : RayBase
    {

        public OneZeroComponent() : base()
        {
            inPort0 = CreateInPort<rayT>();
            inPortList = new InPort[] { inPort0 };
        }

        public OneZeroComponent(OneZeroComponent<rayT> other) : base(other)
        {
            inPort0 = CreatePortFrom<rayT>(other.inPort0);
            inPortList = new InPort[] { inPort0 };
        }


        protected readonly InPort<rayT> inPort0;



    }
}
