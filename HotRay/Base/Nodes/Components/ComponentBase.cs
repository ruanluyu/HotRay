using HotRay.Base.Nodes.Sources;
using HotRay.Base.Port;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components
{
    public abstract class ComponentBase : NodeBase
    {
        public ComponentBase() : base()
        {

        }

        public ComponentBase(ComponentBase other) : base(other)
        {

        }

        public override IReadOnlyList<PortBase> InPorts => SharedEmptyPorts;

        public override IReadOnlyList<PortBase> OutPorts => SharedEmptyPorts;

    }
}
