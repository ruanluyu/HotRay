using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Utils
{
    public class OrGate:ComponentBase
    {

        protected Port<SignalRay>[] inPorts = new Port<SignalRay>[] { new Port<SignalRay>() };
        protected readonly Port<SignalRay> outPort0 = new Port<SignalRay>();

        public OrGate() : base()
        {
            PortNum = 2;
        }

        public OrGate(int count) : base()
        {
            PortNum = count;
        }


        public OrGate(OrGate other) : base(other)
        {
            PortNum = other.PortNum;
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
                    inPorts[i] = new Port<SignalRay>();
                }
            }
        }

        public override IReadOnlyList<PortBase> OutPorts => new PortBase[] { outPort0 };

        public override IReadOnlyList<PortBase> InPorts => inPorts;

        public override NodeBase CloneNode()
        {
            return new OrGate(this);
        }

        public override Status OnPortUpdate(PortBase inport)
        {
            if (PortNum == 0) return Status.Shutdown;
            if(inport.Ray == null)
            {
                for (int i = 0; i < inPorts.Length; i++)
                {
                    if (inPorts[i].Ray != null)
                    {
                        return EmitSignalTo(outPort0, true);
                    }
                }
                return EmitSignalTo(outPort0, false);
            }
            else
            {
                return EmitSignalTo(outPort0, true);
            }
        }

    }
}
