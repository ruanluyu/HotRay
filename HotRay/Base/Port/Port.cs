using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Port
{

    public class Port<rayT> : PortBase where rayT : RayBase
    {

        public Port() { }
        public Port(Port<rayT> other) : base(other) 
        {

        }

        public override Type RayType => typeof(rayT);

        public override PortBase ClonePort()
        {
            var np = new Port<rayT>(this);
            np.TargetPort = null;
            np.SourcePort = null;
            return np;
        }
    }
}
