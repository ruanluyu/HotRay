using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Utils
{
    public class Merge<rayT>:ComponentBase where rayT:RayBase
    {
        protected Port<rayT>[] inPorts = Array.Empty<Port<rayT>>();
        protected readonly Port<rayT> outPort0;

        public Merge() : this(2)
        {
        }

        public Merge(int count) : base()
        {
            PortNum = count;
            outPort0 = CreatePort<rayT>();
        }


        public Merge(Merge<rayT> other) : base(other)
        {
            PortNum = other.PortNum;
            outPort0 = CreatePort<rayT>();
        }

        public int PortNum
        {
            get => inPorts.Length;
            set
            {
                if (value < 0) PortNum = 0;

                if (inPorts.Length == value) return;

                inPorts = new Port<rayT>[value];
                for (int i = 0; i < value; i++)
                {
                    inPorts[i] = CreatePort<rayT>();
                }
            }
        }
        public override IReadOnlyList<PortBase> OutPorts => new PortBase[] { outPort0 };

        public override IReadOnlyList<PortBase> InPorts => inPorts;

        public override NodeBase CloneNode()
        {
            return new Merge<rayT>(this);
        }


        public override Status OnActivated()
        {
            if (PortNum == 0) return Status.Shutdown;
            foreach (var port in inPorts)
            {
                if(port.RayChanged() && port.Ray != null)
                {
                    return EmitRayTo(outPort0, port.Ray);
                }
            }
            return EmitRayTo(outPort0, null);
        }

    }
}
