using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Port
{
    public abstract class InPort : PortBase
    {
        public InPort() { }
        public InPort(InPort other) : base(other)
        {
            SourcePort = null;
        }

        public OutPort? SourcePort { get; set; }

    }


    public class InPort<rayT> : InPort where rayT:RayBase
    {
        public InPort() { }
        public InPort(InPort<rayT> other) : base(other)
        {
            SourcePort = null;
        }

        public override Type RayType => typeof(rayT);

        public override PortBase ClonePort()
        {
            var np = new InPort<rayT>(this);
            return np;
        }
    }
}
