using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Sources
{
    public class OneRaySource<rayT>:SourceBase 
        where rayT: RayBase
    {
        public OneRaySource() : base()
        {

        }

        public OneRaySource(OneRaySource<rayT> other) : base(other)
        {
            
        }


        protected readonly Port<rayT> outPort0 = new Port<rayT>();


        public override IPort[] OutputPorts => new IPort[] { outPort0 };

    }
}
