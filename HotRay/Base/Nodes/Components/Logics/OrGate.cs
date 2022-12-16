using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Logics
{
    public class OrGate : ComponentBase
    {

        protected Port<SignalRay>[] inPorts = Array.Empty<Port<SignalRay>>();
        protected readonly Port<SignalRay> outPort0;

        public OrGate() : base()
        {
            PortNum = 2;
            outPort0 = CreatePort<SignalRay>();
        }

        public OrGate(int count) : base()
        {
            PortNum = count;
            outPort0 = CreatePort<SignalRay>();
        }


        public OrGate(OrGate other) : base(other)
        {
            PortNum = other.PortNum;
            outPort0 = CreatePort<SignalRay>();
        }

        public int PortNum
        {
            get => inPorts.Length;
            set
            {
                if (value < 0) PortNum = 0;

                if (inPorts.Length == value) return;

                inPorts = new Port<SignalRay>[value];
                for (int i = 0; i < value; i++)
                {
                    inPorts[i] = CreatePort<SignalRay>();
                }
            }
        }

        public override IReadOnlyList<PortBase> OutPorts => new PortBase[] { outPort0 };

        public override IReadOnlyList<PortBase> InPorts => inPorts;

        public override NodeBase CloneNode()
        {
            return new OrGate(this);
        }

        public override Status OnActivated()
        {
            if (PortNum == 0) return Status.Shutdown;
            for (int i = 0; i < inPorts.Length; i++)
            {
                if (inPorts[i].Ray != null) return EmitSignalTo(outPort0, true);
            }
            return EmitSignalTo(outPort0, false);
        }

    }
}
