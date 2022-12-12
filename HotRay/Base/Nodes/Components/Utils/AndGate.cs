using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Utils
{
    public class AndGate:ComponentBase
    {
        protected Port<SignalRay>[] inPorts = new Port<SignalRay>[] { new Port<SignalRay>() };
        protected readonly Port<SignalRay> outPort0 = new Port<SignalRay>();

        public AndGate() : base()
        {
            PortNum = 2;
        }

        public AndGate(int count) : base()
        {
            PortNum = count;
        }


        public AndGate(AndGate other) : base(other)
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

        public override IReadOnlyList<IPort> OutPorts => new IPort[] { outPort0 };

        public override IReadOnlyList<IPort> InPorts => inPorts;

        public override INode CloneNode()
        {
            return new AndGate(this);
        }

        public override Status OnPortUpdate(IPort inport)
        {
            if (PortNum == 0) return Status.Shutdown;
            if (inport.Ray == null) return EmitSignalAndShutDown(outPort0, false);
            for (int i = 0; i < inPorts.Length; i++)
            {
                if (inPorts[i].Ray == null)
                {
                    return EmitSignalAndShutDown(outPort0, false);
                }
            }
            return EmitSignalAndShutDown(outPort0, true);
        }

    }
}
